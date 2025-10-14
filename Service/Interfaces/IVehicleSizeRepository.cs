using Common.Models.VehicleSize;

namespace Service.Interfaces;

public interface IVehicleSizeRepository
{
    Task<List<VehicleSizeDto>> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto);
    Task<VehicleSizeDto> GetByIdAsync(Guid id);
}