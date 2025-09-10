namespace M01.BaselineAPIProjectController.Requests;

/// <summary>
/// Represents the request to assign a user to a task.
/// </summary>
public class AssignUserToTaskRequest
{
    /// <summary>
    /// Gets or sets the ID of the user to assign.
    /// </summary>
    public Guid UserId { get; set; }
}