namespace M01.BaselineAPIProjectController.Requests;

/// <summary>
/// Represents the request to create a new project.
/// </summary>
public class CreateProjectRequest
{
    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of the project.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the expected start date of the project.
    /// </summary>
    public DateTime ExpectedStartDate { get; set; }

    /// <summary>
    /// Gets or sets the budget allocated for the project.
    /// </summary>
    public decimal Budget { get; set; }
}