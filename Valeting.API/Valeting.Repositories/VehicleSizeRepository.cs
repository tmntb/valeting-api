using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Valeting.Business.VehicleSize;
using Valeting.Repositories.Entities;
using Valeting.Repositories.Interfaces;

namespace Valeting.Repositories;

public class VehicleSizeRepository(ValetingContext valetingContext, IMapper mapper) : IVehicleSizeRepository
{
    public async Task<VehicleSizeListDTO> GetAsync(VehicleSizeFilterDTO vehicleSizeFilterDTO)
    {
        var vehicleSizeListDTO = new VehicleSizeListDTO() { VehicleSizes = [] };

        var initialList = await valetingContext.RdVehicleSizes.ToListAsync();
        var listVehicleSize = from rdVehicleSize in initialList
                                where (!vehicleSizeFilterDTO.Active.HasValue || rdVehicleSize.Active == vehicleSizeFilterDTO.Active)
                                select rdVehicleSize;

        if (listVehicleSize == null)
            return vehicleSizeListDTO;

        vehicleSizeListDTO.TotalItems = listVehicleSize.Count();
        var nrPages = decimal.Divide(vehicleSizeListDTO.TotalItems, vehicleSizeFilterDTO.PageSize);
        vehicleSizeListDTO.TotalPages = (int)(nrPages - Math.Truncate(nrPages) > 0 ? Math.Truncate(nrPages) + 1 : Math.Truncate(nrPages));

        listVehicleSize = listVehicleSize.OrderBy(x => x.Id);
        listVehicleSize = listVehicleSize.Skip((vehicleSizeFilterDTO.PageNumber - 1) * vehicleSizeFilterDTO.PageSize).Take(vehicleSizeFilterDTO.PageSize);
        vehicleSizeListDTO.VehicleSizes = mapper.Map<List<VehicleSizeDTO>>(listVehicleSize);
        return vehicleSizeListDTO;
    }

    public async Task<VehicleSizeDTO> GetByIDAsync(Guid id)
    {
        var rdVehicleSize = await valetingContext.RdVehicleSizes.FindAsync(id);
        if (rdVehicleSize == null)
            return null;

       return new VehicleSizeDTO()
        {
            Id = id,
            Description = rdVehicleSize.Description,
            Active = rdVehicleSize.Active
        };
    }
}