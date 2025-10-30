using System.Text.Json.Serialization;

namespace Api.Models.VehicleSize;

/// <summary>
/// Represents a vehicle size resource in the API.
/// </summary>
public class VehicleSizeApi
{
    /// <summary>
    /// The unique identifier of the vehicle size.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The description of the vehicle size (e.g., Small, Medium, Large).
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Indicates whether the vehicle size is active.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Contains the hypermedia link for the vehicle size resource.
    /// </summary>
    [JsonPropertyName("_link")]
    public VehicleSizeApiLink Link { get; set; }
}
