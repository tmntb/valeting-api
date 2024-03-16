using Valeting.Services.Objects.Core;

namespace Valeting.Services.Objects.VehicleSize;

public class GetVehicleSizeSVRequest
{
    public Guid Id { get; set; }
}

public class GetVehicleSizeSVResponse : ValetingOutputSV
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
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