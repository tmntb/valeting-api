using Valeting.Common.Models.VehicleSize;

namespace Valeting.Repository.Interfaces;

public interface IVehicleSizeRepository
{
    Task<VehicleSizeListDto> GetAsync(VehicleSizeFilterDto vehicleSizeFilterDto);
    Task<VehicleSizeDto> GetByIdAsync(Guid id);
}