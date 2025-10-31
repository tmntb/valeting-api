namespace Service.Models.VehicleSize;

/// <summary>
/// Represents a vehicle size with its details.
/// </summary>
public class VehicleSizeDto
{
    /// <summary>
    /// The unique identifier of the vehicle size.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The description of the vehicle size.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Indicates whether the vehicle size is active.
    /// </summary>
    public bool Active { get; set; }
}
