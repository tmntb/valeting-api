using Valeting.Business.VehicleSize;

namespace Valeting.Repositories.Interfaces
{
    public interface IVehicleSizeRepository
    {
        Task<VehicleSizeDTO> FindByIDAsync(Guid id);
        Task<VehicleSizeListDTO> ListAsync(VehicleSizeFilterDTO vehicleSizeFilterDTO);
    }
}
