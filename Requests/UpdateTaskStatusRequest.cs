using M01.BaselineAPIProjectController.Enums;

namespace M01.BaselineAPIProjectController.Requests;

/// <summary>
/// Represents the request to update the status of a task.
/// </summary>
public class UpdateTaskStatusRequest
{
    /// <summary>
    /// Gets or sets the new status of the task.
    /// </summary>
    public ProjectTaskStatus Status { get; set; }
}