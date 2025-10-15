using Service.Models.VehicleSize;
using Service.Models.VehicleSize.Payload;

namespace Service.Interfaces;

public interface IVehicleSizeService
{
    Task<VehicleSizeDto> GetByIdAsync(Guid id);
    Task<VehicleSizePaginatedDtoResponse> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto);
}