using Common.Models.VehicleSize;

namespace Service.Interfaces;

public interface IVehicleSizeService
{
    Task<VehicleSizeDto> GetByIdAsync(Guid id);
    Task<VehicleSizePaginatedDtoResponse> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto);
}