using Valeting.Common.Models.Core;

namespace Valeting.Common.Models.VehicleSize;

public class GetVehicleSizeDtoRequest
{
    public Guid Id { get; set; }
}

public class GetVehicleSizeDtoResponse : ValetingOutputDto
{
    public VehicleSizeDto VehicleSize { get; set; }
}

public class PaginatedVehicleSizeDtoRequest
{
    public VehicleSizeFilterDto Filter { get; set; }
}

public class PaginatedVehicleSizeDtoResponse : ValetingOutputDto
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<VehicleSizeDto> VehicleSizes { get; set; }
}