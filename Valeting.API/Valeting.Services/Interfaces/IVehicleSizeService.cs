using Valeting.Business;

namespace Valeting.Services.Interfaces
{
    public interface IVehicleSizeService
    {
        Task<VehicleSizeDTO> FindByIDAsync(Guid id);
        Task<IEnumerable<VehicleSizeDTO>> ListAllAsync();
    }
}

