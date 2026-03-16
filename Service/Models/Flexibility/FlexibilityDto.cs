namespace Service.Models.Flexibility;

/// <summary>
/// Represents a flexibility entity.
/// </summary>
public class FlexibilityDto
{
    /// <summary>
    /// Unique identifier of the flexibility.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Description of the flexibility.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The number of minutes that the flexibility represents (e.g., 1440 for +/- 1 day).
    /// </summary>
    public int NumberOfMinutes { get; set; }

    /// <summary>
    /// Indicates whether the flexibility is active.
    /// </summary>
    public bool Active { get; set; }
}
