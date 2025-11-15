namespace Api.Models.VehicleSize.Payload;

/// <summary>
/// Represents a response containing a single vehicle size.
/// </summary>
public class VehicleSizeApiResponse
{
    /// <summary>
    /// The vehicle size returned by the API.
    /// </summary>
    public VehicleSizeApi VehicleSize { get; set; }
}
