using Valeting.Core.Models.Core;

namespace Valeting.Core.Models.VehicleSize;

public class GetVehicleSizeSVRequest
{
    public Guid Id { get; set; }
}

public class GetVehicleSizeSVResponse : ValetingOutputSV
{
    public VehicleSizeSV VehicleSize { get; set; }
}

public class PaginatedVehicleSizeSVRequest
{
    public VehicleSizeFilterSV Filter { get; set; }
}

public class PaginatedVehicleSizeSVResponse : ValetingOutputSV
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<VehicleSizeSV> VehicleSizes { get; set; }
}