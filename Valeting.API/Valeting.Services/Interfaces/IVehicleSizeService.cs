using Valeting.Business.VehicleSize;

namespace Valeting.Services.Interfaces
{
    public interface IVehicleSizeService
    {
        Task<VehicleSizeDTO> FindByIDAsync(Guid id);
        Task<VehicleSizeListDTO> ListAllAsync(VehicleSizeFilterDTO vehicleSizeFilterDTO);
    }
}

