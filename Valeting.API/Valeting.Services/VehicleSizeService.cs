using Valeting.Business;
using Valeting.Services.Interfaces;
using Valeting.Repositories.Interfaces;

namespace Valeting.Service
{
    public class VehicleSizeService : IVehicleSizeService
    {
        private readonly IVehicleSizeRepository _vehicleSizeRepository;

        public VehicleSizeService(IVehicleSizeRepository vehicleSizeRepository)
        {
            _vehicleSizeRepository = vehicleSizeRepository ?? throw new Exception("vehicleSizeRepository is null");
        }

        public async Task<VehicleSizeDTO> FindByIDAsync(Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new Exception("FlexibilityId is empty");

            var vehicleSizeDTO = await _vehicleSizeRepository.FindByIDAsync(id);
            if (vehicleSizeDTO == null)
                throw new Exception("Booking not found");

            return vehicleSizeDTO;
        }

        public async Task<IEnumerable<VehicleSizeDTO>> ListAllAsync()
        {
            var vehicleSizeDTOs = await _vehicleSizeRepository.ListAsync();
            if (vehicleSizeDTOs == null || !vehicleSizeDTOs.Any())
                throw new Exception("None vehicle size was found");

            return vehicleSizeDTOs;
        }
    }
}
