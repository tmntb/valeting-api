using DJValeting.Business;

namespace DJValeting.Repositories.Interfaces
{
    public interface IVehicleSizeRepository
    {
        Task<VehicleSizeDTO> FindByIDAsync(Guid id);
        Task<IEnumerable<VehicleSizeDTO>> ListAsync();
    }
}
