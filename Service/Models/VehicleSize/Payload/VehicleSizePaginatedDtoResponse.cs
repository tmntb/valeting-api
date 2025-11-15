namespace Service.Models.VehicleSize.Payload;

/// <summary>
/// Represents a paginated response containing vehicle size data.
/// </summary>
public class VehicleSizePaginatedDtoResponse
{
    /// <summary>
    /// Total number of vehicle size items available.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total number of pages based on the page size.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// The list of vehicle sizes in the current page.
    /// </summary>
    public List<VehicleSizeDto> VehicleSizes { get; set; }
}
