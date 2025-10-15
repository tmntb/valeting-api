namespace Service.Models.VehicleSize.Payload;

public class VehicleSizePaginatedDtoResponse
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<VehicleSizeDto> VehicleSizes { get; set; }
}