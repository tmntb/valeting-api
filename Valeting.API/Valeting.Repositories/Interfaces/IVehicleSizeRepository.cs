using Valeting.Business.VehicleSize;

namespace Valeting.Repositories.Interfaces;

public interface IVehicleSizeRepository
{
    Task<VehicleSizeListDTO> GetAsync(VehicleSizeFilterDTO vehicleSizeFilterDTO);
    Task<VehicleSizeDTO> GetByIDAsync(Guid id);
}