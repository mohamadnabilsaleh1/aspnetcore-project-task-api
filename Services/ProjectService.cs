using M01.BaselineAPIProjectController.Data;
using M01.BaselineAPIProjectController.Entities;
using M01.BaselineAPIProjectController.Enums;
using M01.BaselineAPIProjectController.Requests;
using M01.BaselineAPIProjectController.Responses;
using M01.BaselineAPIProjectController.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace M01.BaselineAPIProjectController.Services;

public class ProjectService(AppDbContext context) : IProjectService
{
    private readonly AppDbContext _context = context;

    public async Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request, Guid currentUserId)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ExpectedStartDate = request.ExpectedStartDate,
            Budget = request.Budget,
            OwnerId = currentUserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return ProjectResponse.FromEntity(project);
    }

    public async Task<List<ProjectResponse>> GetProjectsAsync()
    {
        return await _context.Projects
            .AsNoTracking()
            .Include(p => p.Tasks)
            .Select(p => ProjectResponse.FromEntity(p))
            .ToListAsync();
    }

    public async Task<ProjectResponse?> GetProjectAsync(Guid projectId)
    {
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        return project is null ? null : ProjectResponse.FromEntity(project);
    }

    public async Task UpdateProjectAsync(Guid projectId, UpdateProjectRequest request, Guid currentUserId)
    {
        var project = await _context.Projects.FindAsync(projectId)
            ?? throw new BusinessRuleException("Project not found", StatusCodes.Status404NotFound);

        if (project.OwnerId != currentUserId)
            throw new BusinessRuleException("Only the project owner can update the project.", StatusCodes.Status403Forbidden);

        if (project.ActualEndDate.HasValue)
            throw new BusinessRuleException("Cannot modify an ended project.", StatusCodes.Status409Conflict);

        project.Name = request.Name;
        project.Description = request.Description;
        project.ExpectedStartDate = request.ExpectedStartDate;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteProjectAsync(Guid projectId, Guid currentUserId)
    {
        var project = await _context.Projects.FindAsync(projectId)
            ?? throw new BusinessRuleException("Project not found", StatusCodes.Status404NotFound);

        if (project.OwnerId != currentUserId)
            throw new BusinessRuleException("Only the project owner can delete the project.", StatusCodes.Status403Forbidden);

        if (project.ActualEndDate.HasValue)
            throw new BusinessRuleException("Cannot delete an ended project.", StatusCodes.Status409Conflict);

        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();
    }

    public async Task ManageBudgetAsync(Guid projectId, UpdateBudgetRequest request, Guid currentUserId)
    {
        var project = await _context.Projects.FindAsync(projectId)
            ?? throw new BusinessRuleException("Project not found", StatusCodes.Status404NotFound);

        if (project.OwnerId != currentUserId)
            throw new BusinessRuleException("Only the project owner can manage the budget.", StatusCodes.Status403Forbidden);

        if (project.ActualEndDate.HasValue)
            throw new BusinessRuleException("Cannot modify an ended project.", StatusCodes.Status409Conflict);

        project.Budget = request.Budget;

        await _context.SaveChangesAsync();

        await Task.CompletedTask;
    }

    public async Task<ProjectTaskResponse> CreateTaskAsync(Guid projectId, CreateTaskRequest request, Guid currentUserId)
    {
        var project = await _context.Projects.FindAsync(projectId)
            ?? throw new BusinessRuleException("Project not found", StatusCodes.Status404NotFound);

        if (project.OwnerId != currentUserId)
            throw new BusinessRuleException("Only the project owner can create tasks.", StatusCodes.Status403Forbidden);

        if (project.ActualEndDate.HasValue)
            throw new BusinessRuleException("Cannot modify tasks on an ended project.", StatusCodes.Status409Conflict);

        var task = new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            AssignedUserId = request.AssignedUserId,
            ProjectId = projectId,
            CreatedAt = DateTime.UtcNow,
            Status = ProjectTaskStatus.NotStarted
        };

        _context.ProjectTasks.Add(task);
        await _context.SaveChangesAsync();

        return ProjectTaskResponse.FromEntity(task);
    }

    public async Task<ProjectTaskResponse> GetTaskAsync(Guid projectId, Guid taskId)
    {
        var task = await _context.ProjectTasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId)
            ?? throw new BusinessRuleException("Task not found", StatusCodes.Status404NotFound);

        return ProjectTaskResponse.FromEntity(task);
    }

    public async Task UpdateTaskStatusAsync(Guid projectId, Guid taskId, UpdateTaskStatusRequest request, Guid currentUserId)
    {
        var task = await _context.ProjectTasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId)
            ?? throw new BusinessRuleException("Task not found", StatusCodes.Status404NotFound);

        if (task.AssignedUserId != currentUserId && task.Project.OwnerId != currentUserId)
            throw new BusinessRuleException("Only assigned user or project owner can update task status.", StatusCodes.Status403Forbidden);

        if (task.Project.ActualEndDate.HasValue)
            throw new BusinessRuleException("Cannot modify tasks on an ended project.", StatusCodes.Status409Conflict);

        task.Status = request.Status;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(Guid projectId, Guid taskId, Guid currentUserId)
    {
        var task = await _context.ProjectTasks.Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId)
            ?? throw new BusinessRuleException("Task not found", StatusCodes.Status404NotFound);

        if (task.Project.OwnerId != currentUserId)
            throw new BusinessRuleException("Only the project owner can delete the task.", StatusCodes.Status403Forbidden);

        if (task.Project.ActualEndDate.HasValue)
            throw new BusinessRuleException("Cannot delete tasks from an ended project.", StatusCodes.Status409Conflict);

        _context.ProjectTasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task AssignUserToTaskAsync(Guid projectId, Guid taskId, AssignUserToTaskRequest request, Guid currentUserId)
    {
        var task = await _context.ProjectTasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId)
            ?? throw new BusinessRuleException("Task not found", StatusCodes.Status404NotFound);

        if (task.Project.OwnerId != currentUserId)
            throw new BusinessRuleException("Only the project owner can assign users to tasks.", StatusCodes.Status403Forbidden);

        if (task.Project.ActualEndDate.HasValue)
            throw new BusinessRuleException("Cannot assign users to tasks in an ended project.", StatusCodes.Status409Conflict);

        task.AssignedUserId = request.UserId;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateTaskAsync(Guid projectId, Guid taskId, UpdateTaskRequest request, Guid currentUserId)
    {
        var task = await _context.ProjectTasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId)
            ?? throw new BusinessRuleException("Task not found", StatusCodes.Status404NotFound);

        if (task.AssignedUserId != currentUserId && task.Project.OwnerId != currentUserId)
            throw new BusinessRuleException("Only assigned user or project owner can update the task.", StatusCodes.Status403Forbidden);

        if (task.Project.ActualEndDate.HasValue)
            throw new BusinessRuleException("Cannot update tasks in an ended project.", StatusCodes.Status409Conflict);

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;

        await _context.SaveChangesAsync();
    }

    public async Task EndProjectAsync(Guid projectId, Guid currentUserId)
    {
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId)
            ?? throw new BusinessRuleException("Project not found", StatusCodes.Status404NotFound);

        if (project.OwnerId != currentUserId)
            throw new BusinessRuleException("Only the project owner can end the project.", StatusCodes.Status403Forbidden);

        if (project.ActualEndDate.HasValue)
            throw new BusinessRuleException("Project is already ended.", StatusCodes.Status409Conflict);

        var allTasksClosed = project.Tasks.All(t =>
            t.Status == ProjectTaskStatus.Completed || t.Status == ProjectTaskStatus.Cancelled);

        if (!allTasksClosed)
            throw new BusinessRuleException("Cannot end project with active tasks.", StatusCodes.Status409Conflict);

        project.ActualEndDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}
