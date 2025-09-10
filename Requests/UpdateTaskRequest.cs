using System.Text.Json.Serialization;
using M01.BaselineAPIProjectController.Enums;

namespace M01.BaselineAPIProjectController.Requests;

/// <summary>
/// Represents the request to update an existing task.
/// </summary>

public class UpdateTaskRequest
{
    /// <summary>
    /// Gets or sets the updated title of the task.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the updated description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the updated status of the task.
    /// </summary>
    public ProjectTaskStatus Status { get; set; }
}