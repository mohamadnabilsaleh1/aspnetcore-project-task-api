using M01.BaselineAPIProjectController.Entities;

namespace M01.BaselineAPIProjectController.Responses;

public class ProjectTaskResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public Guid AssignedUserId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public static ProjectTaskResponse FromEntity(ProjectTask task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        AssignedUserId = task.AssignedUserId,
        Status = task.Status.ToString(),
        CreatedAt = task.CreatedAt
    };
}