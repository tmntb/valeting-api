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
    public string Description { get; set; }

    /// <summary>
    /// Indicates whether the flexibility is active.
    /// </summary>
    public bool Active { get; set; }
}
