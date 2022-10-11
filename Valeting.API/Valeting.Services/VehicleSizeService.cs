using Valeting.Business;
using Valeting.Repositories.Interfaces;
using Valeting.Services.Interfaces;

namespace Valeting.Service
{
    public class VehicleSizeService : IVehicleSizeService
    {
        private IVehicleSizeRepository _vehicleSizeRepository;

        public VehicleSizeService(IVehicleSizeRepository vehicleSizeRepository)
        {
            _vehicleSizeRepository = vehicleSizeRepository ?? throw new Exception("vehicleSizeRepository is null");
        }

        public async Task<VehicleSizeDTO> FindByIDAsync(Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new Exception("FlexibilityId is empty");

            VehicleSizeDTO vehicleSizeDTO = await _vehicleSizeRepository.FindByIDAsync(id);
            if (vehicleSizeDTO == null)
                throw new Exception("Booking not found");

            return vehicleSizeDTO;
        }

        public async Task<IEnumerable<VehicleSizeDTO>> ListAllAsync()
        {
            IEnumerable<VehicleSizeDTO> vehicleSizeDTOs = await _vehicleSizeRepository.ListAsync();
            if (vehicleSizeDTOs == null || !vehicleSizeDTOs.Any())
                throw new Exception("None vehicle size was found");

            return vehicleSizeDTOs;
        }
    }
}
