using Service.Models.VehicleSize;
using Service.Models.VehicleSize.Payload;

namespace Service.Interfaces;

public interface IVehicleSizeRepository
{
    Task<VehicleSizeDto> GetByIdAsync(Guid id);
    Task<List<VehicleSizeDto>> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto);
}