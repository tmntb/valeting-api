using Valeting.Repository.Models.VehicleSize;

namespace Valeting.Repository.Repositories.Interfaces;

public interface IVehicleSizeRepository
{
    Task<VehicleSizeListDTO> GetAsync(VehicleSizeFilterDTO vehicleSizeFilterDTO);
    Task<VehicleSizeDTO> GetByIDAsync(Guid id);
}