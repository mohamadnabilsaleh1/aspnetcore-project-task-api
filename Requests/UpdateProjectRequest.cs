namespace M01.BaselineAPIProjectController.Requests;

/// <summary>
/// Represents the request to update project details.
/// </summary>
public class UpdateProjectRequest
{
    /// <summary>
    /// Gets or sets the new name of the project.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the updated expected start date of the project.
    /// </summary>
    public DateTime ExpectedStartDate { get; set; }

    /// <summary>
    /// Gets or sets the updated description of the project.
    /// </summary>
    public string? Description { get; set; }
}