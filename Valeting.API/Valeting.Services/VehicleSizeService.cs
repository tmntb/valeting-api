using Valeting.Common.Messages;
using Valeting.Common.Exceptions;
using Valeting.Services.Interfaces;
using Valeting.Business.VehicleSize;
using Valeting.Repositories.Interfaces;

namespace Valeting.Service;

public class VehicleSizeService(IVehicleSizeRepository vehicleSizeRepository) : IVehicleSizeService
{
    public async Task<VehicleSizeDTO> FindByIDAsync(Guid id)
    {
        if (id.Equals(Guid.Empty))
            throw new InputException(Messages.InvalidVehicleSizeId);

        var vehicleSizeDTO = await vehicleSizeRepository.FindByIDAsync(id);
        if (vehicleSizeDTO == null)
            throw new NotFoundException(Messages.VehicleSizeNotFound);

        return vehicleSizeDTO;
    }

    public async Task<VehicleSizeListDTO> ListAllAsync(VehicleSizeFilterDTO vehicleSizeFilterDTO)
    {
        if (vehicleSizeFilterDTO.PageNumber == 0)
            throw new InputException(Messages.InvalidPageNumber);

        return await vehicleSizeRepository.ListAsync(vehicleSizeFilterDTO);
    }
}