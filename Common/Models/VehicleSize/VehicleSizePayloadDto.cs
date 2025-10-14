namespace Common.Models.VehicleSize;

public class GetVehicleSizeDtoRequest
{
    public Guid Id { get; set; }
}

public class GetVehicleSizeDtoResponse
{
    public VehicleSizeDto VehicleSize { get; set; }
}

public class PaginatedVehicleSizeDtoRequest
{
    public VehicleSizeFilterDto Filter { get; set; }
}

public class VehicleSizePaginatedDtoResponse
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<VehicleSizeDto> VehicleSizes { get; set; }
}