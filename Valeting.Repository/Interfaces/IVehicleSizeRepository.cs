using Valeting.Common.Models.VehicleSize;

namespace Valeting.Repository.Interfaces;

public interface IVehicleSizeRepository
{
    Task<List<VehicleSizeDto>> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto);
    Task<VehicleSizeDto> GetByIdAsync(Guid id);
}