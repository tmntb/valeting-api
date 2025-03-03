using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Valeting.Repository.Entities;
using Valeting.Repository.Interfaces;
using Valeting.Common.Models.VehicleSize;

namespace Valeting.Repository.Repositories;

public class VehicleSizeRepository(ValetingContext valetingContext, IMapper mapper) : IVehicleSizeRepository
{
    public async Task<VehicleSizeListDto> GetAsync(VehicleSizeFilterDto vehicleSizeFilterDto)
    {
        var vehicleSizeListDto = new VehicleSizeListDto() { VehicleSizes = [] };

        var initialList = await valetingContext.RdVehicleSizes.ToListAsync();
        var listVehicleSize = from rdVehicleSize in initialList
                                where !vehicleSizeFilterDto.Active.HasValue || rdVehicleSize.Active == vehicleSizeFilterDto.Active
                                select rdVehicleSize;

        if (listVehicleSize == null)
            return vehicleSizeListDto;

        vehicleSizeListDto.TotalItems = listVehicleSize.Count();
        var nrPages = decimal.Divide(vehicleSizeListDto.TotalItems, vehicleSizeFilterDto.PageSize);
        vehicleSizeListDto.TotalPages = (int)(nrPages - Math.Truncate(nrPages) > 0 ? Math.Truncate(nrPages) + 1 : Math.Truncate(nrPages));

        listVehicleSize = listVehicleSize.OrderBy(x => x.Id);
        listVehicleSize = listVehicleSize.Skip((vehicleSizeFilterDto.PageNumber - 1) * vehicleSizeFilterDto.PageSize).Take(vehicleSizeFilterDto.PageSize);
        vehicleSizeListDto.VehicleSizes = mapper.Map<List<VehicleSizeDto>>(listVehicleSize);
        return vehicleSizeListDto;
    }

    public async Task<VehicleSizeDto> GetByIdAsync(Guid id)
    {
        var rdVehicleSize = await valetingContext.RdVehicleSizes.FindAsync(id);
        if (rdVehicleSize == null)
            return null;

       return new VehicleSizeDto()
        {
            Id = id,
            Description = rdVehicleSize.Description,
            Active = rdVehicleSize.Active
        };
    }
}