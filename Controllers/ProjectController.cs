using Asp.Versioning;
using M01.BaselineAPIProjectController.Permissions;
using M01.BaselineAPIProjectController.Requests;
using M01.BaselineAPIProjectController.Responses;
using M01.BaselineAPIProjectController.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace M01.BaselineAPIProjectController.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/projects")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Tags("Projects")]
public class ProjectController(IProjectService projectService) : ControllerBase
{
    private Guid GetUserId()
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HttpPost]
    [MapToApiVersion("1.0")]
    [Authorize(Permission.Project.Create)]
    [Consumes("application/json")]
    [ProducesResponseType<ProjectResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("CreateProjectV1")]
    [EndpointSummary("Creates a new project")]
    [EndpointDescription("Creates a new project for the current user and returns the created result.")]
    public async Task<ActionResult<ProjectResponse>> CreateProject([FromBody] CreateProjectRequest request)
    {
        var userId = GetUserId();
        var result = await projectService.CreateProjectAsync(request, userId);

        return CreatedAtAction(
            nameof(GetProject),
            new { projectId = result.Id },
            result);
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [Authorize(Permission.Project.Read)]
    [ProducesResponseType<List<ProjectResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetProjectsV1")]
    [EndpointSummary("Retrieves all projects")]
    [EndpointDescription("Retrieves all projects owned or accessible by the user.")]
    public async Task<ActionResult<List<ProjectResponse>>> GetProjects()
    {
        var projects = await projectService.GetProjectsAsync();
        return Ok(projects);
    }

    [HttpGet]
    [MapToApiVersion("2.0")]
    [Authorize(Permission.Project.Read)]
    [ProducesResponseType<List<ProjectResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetProjectsV2")]
    [EndpointSummary("Retrieves all projects with currency info")]
    [EndpointDescription("Retrieves all projects and includes a currency field for each.")]
    public async Task<ActionResult<List<ProjectResponse>>> GetProjectsV2()
    {
        var projects = await projectService.GetProjectsAsync();
        foreach (var project in projects)
            project.Currency = "USD";

        return Ok(projects);
    }

    [HttpGet("{projectId:guid}")]
    [MapToApiVersion("1.0")]
    [Authorize(Permission.Project.Read)]
    [ProducesResponseType<ProjectResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetProjectV1")]
    [EndpointSummary("Retrieves a specific project")]
    [EndpointDescription("Retrieves a specific project by its ID.")]
    public async Task<ActionResult<ProjectResponse>> GetProject([FromRoute] Guid projectId)
    {
        var project = await projectService.GetProjectAsync(projectId);
        return Ok(project);
    }

    [HttpGet("{projectId:guid}")]
    [MapToApiVersion("2.0")]
    [Authorize(Permission.Project.Read)]
    [ProducesResponseType<ProjectResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetProjectV2")]
    [EndpointSummary("Retrieves a specific project with currency info")]
    [EndpointDescription("Retrieves a project by ID and includes currency information.")]
    public async Task<ActionResult<ProjectResponse>> GetProjectV2([FromRoute] Guid projectId)
    {
        var project = await projectService.GetProjectAsync(projectId);
        if (project is null)
            return NotFound("Project was not found");

        project.Currency = "USD";
        return Ok(project);
    }

    [HttpPut("{projectId:guid}")]
    [MapToApiVersion("1.0")]
    [Authorize(Permission.Project.Update)]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateProjectV1")]
    [EndpointSummary("Updates a project")]
    [EndpointDescription("Updates the data of an existing project.")]
    public async Task<IActionResult> UpdateProject([FromRoute] Guid projectId, [FromBody] UpdateProjectRequest request)
    {
        await projectService.UpdateProjectAsync(projectId, request, GetUserId());
        return NoContent();
    }

    [HttpDelete("{projectId:guid}")]
    [MapToApiVersion("1.0")]
    [Authorize(Permission.Project.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeleteProjectV1")]
    [EndpointSummary("Deletes a project")]
    [EndpointDescription("Deletes a specific project by ID.")]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid projectId)
    {
        await projectService.DeleteProjectAsync(projectId, GetUserId());
        return NoContent();
    }

    [HttpPut("{projectId:guid}/budget")]
    [MapToApiVersion("1.0")]
    [Authorize(Permission.Project.ManageBudget)]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateBudgetV1")]
    [EndpointSummary("Updates project budget")]
    [EndpointDescription("Updates the budget for a specific project.")]
    public async Task<IActionResult> UpdateBudget([FromRoute] Guid projectId, [FromBody] UpdateBudgetRequest request)
    {
        await projectService.ManageBudgetAsync(projectId, request, GetUserId());
        return NoContent();
    }

    [HttpPut("{projectId:guid}/completion")]
    [MapToApiVersion("1.0")]
    [Authorize(Permission.Project.Update)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("EndProjectV1")]
    [EndpointSummary("Marks project as completed")]
    [EndpointDescription("Marks a project as completed.")]
    public async Task<IActionResult> EndProject([FromRoute] Guid projectId)
    {
        await projectService.EndProjectAsync(projectId, GetUserId());
        return NoContent();
    }

    // === TASK ENDPOINTS ===

    [HttpPost("{projectId:guid}/tasks")]
    [MapToApiVersion("1.0")]
    [Authorize(Permission.Task.Create)]
    [Consumes("application/json")]
    [ProducesResponseType<ProjectTaskResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [Tags("Tasks")]
    [EndpointName("CreateTaskV1")]
    [EndpointSummary("Creates a task in a project")]
    [EndpointDescription("Creates a task under the given project.")]
    public async Task<ActionResult<ProjectTaskResponse>> CreateTask([FromRoute] Guid projectId, [FromBody] CreateTaskRequest request)
    {
        var task = await projectService.CreateTaskAsync(projectId, request, GetUserId());
        return CreatedAtAction(nameof(GetTask), new { projectId, taskId = task.Id }, task);
    }

    [HttpGet("{projectId:guid}/tasks/{taskId:guid}")]
    [MapToApiVersion("1.0")]
    [Authorize(Permission.Task.Read)]
    [ProducesResponseType<ProjectTaskResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [Tags("Tasks")]
    [EndpointName("GetTaskV1")]
    [EndpointSummary("Gets a task by ID")]
    [EndpointDescription("Retrieves a specific task from a project.")]
    public async Task<ActionResult<ProjectTaskResponse>> GetTask([FromRoute] Guid projectId, [FromRoute] Guid taskId)
    {
        var task = await projectService.GetTaskAsync(projectId, taskId);
        return Ok(task);
    }

    [HttpPut("{projectId:guid}/tasks/{taskId:guid}/status")]
    [MapToApiVersion("1.0")]
    [Authorize(Permission.Task.UpdateStatus)]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [Tags("Tasks")]
    [EndpointName("UpdateTaskStatusV1")]
    [EndpointSummary("Updates task status")]
    [EndpointDescription("Updates the status of a task in a project.")]
    public async Task<IActionResult> UpdateTaskStatus([FromRoute] Guid projectId, [FromRoute] Guid taskId, [FromBody] UpdateTaskStatusRequest request)
    {
        await projectService.UpdateTaskStatusAsync(projectId, taskId, request, GetUserId());
        return NoContent();
    }

    [HttpPut("{projectId:guid}/tasks/{taskId:guid}")]
    [MapToApiVersion("1.0")]
    [Authorize(Permission.Task.Update)]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [Tags("Tasks")]
    [EndpointName("UpdateTaskV1")]
    [EndpointSummary("Updates task details")]
    [EndpointDescription("Updates the fields of an existing task.")]
    public async Task<IActionResult> UpdateTask([FromRoute] Guid projectId, [FromRoute] Guid taskId, [FromBody] UpdateTaskRequest request)
    {
        await projectService.UpdateTaskAsync(projectId, taskId, request, GetUserId());
        return NoContent();
    }

    [HttpPut("{projectId:guid}/tasks/{taskId:guid}/assignment")]
    [MapToApiVersion("1.0")]
    [Authorize(Permission.Task.AssignUser)]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [Tags("Tasks")]
    [EndpointName("AssignUserV1")]
    [EndpointSummary("Assigns a user to a task")]
    [EndpointDescription("Assigns a user to a task within a project.")]
    public async Task<IActionResult> AssignUser([FromRoute] Guid projectId, [FromRoute] Guid taskId, [FromBody] AssignUserToTaskRequest request)
    {
        await projectService.AssignUserToTaskAsync(projectId, taskId, request, GetUserId());
        return NoContent();
    }

    [HttpDelete("{projectId:guid}/tasks/{taskId:guid}")]
    [MapToApiVersion("1.0")]
    [Authorize(Permission.Task.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [Tags("Tasks")]
    [EndpointName("DeleteTaskV1")]
    [EndpointSummary("Deletes a task")]
    [EndpointDescription("Deletes a task from a specific project.")]
    public async Task<IActionResult> DeleteTask([FromRoute] Guid projectId, [FromRoute] Guid taskId)
    {
        await projectService.DeleteTaskAsync(projectId, taskId, GetUserId());
        return NoContent();
    }
}