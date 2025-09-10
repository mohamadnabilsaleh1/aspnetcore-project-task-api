using M01.BaselineAPIProjectController.Enums;

namespace M01.BaselineAPIProjectController.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpectedStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public decimal Budget { get; set; }

    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}

public class ProjectTask
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public Guid AssignedUserId { get; set; }
    public ProjectTaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public Project Project { get; set; } = null!;
}