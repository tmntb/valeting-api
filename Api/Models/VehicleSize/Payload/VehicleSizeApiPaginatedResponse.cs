using Api.Models.Core;

namespace Api.Models.VehicleSize.Payload;

/// <summary>
/// Represents a paginated response containing a list of vehicle sizes.
/// </summary>
public class VehicleSizeApiPaginatedResponse : PaginationApi
{
    /// <summary>
    /// The collection of vehicle sizes for the current page.
    /// </summary>
    public List<VehicleSizeApi> VehicleSizes { get; set; }
}
