using Valeting.Common.Exceptions;
using Valeting.Services.Interfaces;
using Valeting.Business.VehicleSize;
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
                throw new InputException("VehicleSizeId is empty");

            var vehicleSizeDTO = await _vehicleSizeRepository.FindByIDAsync(id);
            if (vehicleSizeDTO == null)
                throw new NotFoundException("Vehicle Size not found");

            return vehicleSizeDTO;
        }

        public async Task<VehicleSizeListDTO> ListAllAsync(VehicleSizeFilterDTO vehicleSizeFilterDTO)
        {
            if (vehicleSizeFilterDTO.PageNumber == 0)
                throw new InputException("pageNumber é 0");

            return await _vehicleSizeRepository.ListAsync(vehicleSizeFilterDTO);
        }
    }
}
