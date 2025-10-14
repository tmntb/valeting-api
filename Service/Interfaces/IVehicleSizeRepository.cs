using Common.Models.VehicleSize;

namespace Service.Interfaces;

public interface IVehicleSizeRepository
{
    Task<VehicleSizeDto> GetByIdAsync(Guid id);
    Task<List<VehicleSizeDto>> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto);
}