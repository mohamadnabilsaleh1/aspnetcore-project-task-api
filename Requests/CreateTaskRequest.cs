namespace M01.BaselineAPIProjectController.Requests;

/// <summary>
/// Represents the request to create a new task within a project.
/// </summary>
public class CreateTaskRequest
{
    /// <summary>
    /// Gets or sets the title of the task.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user assigned to the task.
    /// </summary>
    public Guid AssignedUserId { get; set; }
}