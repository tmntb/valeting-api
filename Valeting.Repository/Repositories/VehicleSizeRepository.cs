using Microsoft.EntityFrameworkCore;
using Valeting.Repository.Entities;
using Valeting.Repository.Interfaces;
using Valeting.Common.Models.VehicleSize;

namespace Valeting.Repository.Repositories;

public class VehicleSizeRepository(ValetingContext valetingContext) : IVehicleSizeRepository
{
    public async Task<List<VehicleSizeDto>> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto)
    {
        var initialList = await valetingContext.RdVehicleSizes.ToListAsync();
        var listVehicleSize = from rdVehicleSize in initialList
                              where !vehicleSizeFilterDto.Active.HasValue || rdVehicleSize.Active == vehicleSizeFilterDto.Active
                              select rdVehicleSize;

        return new List<VehicleSizeDto>();
    }

    public async Task<VehicleSizeDto> GetByIdAsync(Guid id)
    {
        var rdVehicleSize = await valetingContext.RdVehicleSizes.FindAsync(id);
        if (rdVehicleSize == null)
            return null;

        return new VehicleSizeDto { };
    }
}