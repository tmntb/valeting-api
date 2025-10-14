using Valeting.Common.Models.VehicleSize;

namespace Valeting.Core.Interfaces;

public interface IVehicleSizeRepository
{
    Task<List<VehicleSizeDto>> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto);
    Task<VehicleSizeDto> GetByIdAsync(Guid id);
}