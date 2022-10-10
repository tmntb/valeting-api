using Valeting.Business;

namespace Valeting.Repositories.Interfaces
{
    public interface IVehicleSizeRepository
    {
        Task<VehicleSizeDTO> FindByIDAsync(Guid id);
        Task<IEnumerable<VehicleSizeDTO>> ListAsync();
    }
}
