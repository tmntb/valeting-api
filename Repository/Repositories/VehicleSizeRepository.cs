using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.VehicleSize;
using Service.Models.VehicleSize.Payload;

namespace Repository.Repositories;

public class VehicleSizeRepository(ValetingContext valetingContext) : IVehicleSizeRepository
{
    public async Task<List<VehicleSizeDto>> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto)
    {
        var initialList = await valetingContext.RdVehicleSizes.ToListAsync();
        var listVehicleSize = from rdVehicleSize in initialList
                              where !vehicleSizeFilterDto.Active.HasValue || rdVehicleSize.Active == vehicleSizeFilterDto.Active
                              select rdVehicleSize;

        return listVehicleSize.Select(x =>
           new VehicleSizeDto
           {
               Id = x.Id,
               Description = x.Description,
               Active = x.Active
           }
       ).ToList();
    }

    public async Task<VehicleSizeDto> GetByIdAsync(Guid id)
    {
        var rdVehicleSize = await valetingContext.RdVehicleSizes.FindAsync(id);
        if (rdVehicleSize == null)
            return null;

        return new() 
        {
            Id = rdVehicleSize.Id,
            Description = rdVehicleSize.Description,
            Active = rdVehicleSize.Active
        };
    }
}