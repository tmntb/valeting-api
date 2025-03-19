using Valeting.Repository.Interfaces;

namespace Valeting.Core.Validators.Utils;

public class ValidationHelpers(IFlexibilityRepository flexibilityRepository, IVehicleSizeRepository vehicleSizeRepository)
{
    public async Task<bool> FlexibilityIsValid(Guid flexibilityId, CancellationToken cancellationToken)
    {
        var result = await flexibilityRepository.GetByIdAsync(flexibilityId);
        return result != null;
    }

    public async Task<bool> VehicleSizeIsValid(Guid vehicleSizeId, CancellationToken cancellationToken)
    {
        var result = await vehicleSizeRepository.GetByIdAsync(vehicleSizeId);
        return result != null;
    }
}