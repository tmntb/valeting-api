using System.Text.Json.Serialization;

namespace Api.Models.Flexibility;

/// <summary>
/// Represents a flexibility option for bookings.
/// </summary>
public class FlexibilityApi
{
    /// <summary>
    /// Unique identifier of the flexibility.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Description of the flexibility option.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Indicating whether the flexibility is active.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// HATEOAS link for this flexibility.
    /// </summary>
    [JsonPropertyName("_link")]
    public FlexibilityApiLink Link { get; set; }
}
