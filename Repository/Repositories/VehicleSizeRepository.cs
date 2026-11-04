using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.VehicleSize;
using Service.Models.VehicleSize.Payload;

namespace Repository.Repositories;

public class VehicleSizeRepository(ValetingContext valetingContext) : IVehicleSizeRepository
{
    /// <inheritdoc />
    public async Task<VehicleSizeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rdVehicleSize = await valetingContext.RdVehicleSizes.FindAsync(id, cancellationToken);
        if (rdVehicleSize == null)
            return null;

        return RdVehicleSize.FromEntity(rdVehicleSize);
    }
    
    /// <inheritdoc />
    public async Task<List<VehicleSizeDto>> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto, CancellationToken cancellationToken = default)
    {
        var initialList = await valetingContext.RdVehicleSizes.ToListAsync(cancellationToken);
        var listVehicleSize = from rdVehicleSize in initialList
                              where !vehicleSizeFilterDto.Active.HasValue || rdVehicleSize.Active == vehicleSizeFilterDto.Active
                              select rdVehicleSize;

        return listVehicleSize.Select(RdVehicleSize.FromEntity).ToList();
    }
}