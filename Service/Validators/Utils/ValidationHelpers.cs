using Service.Interfaces;

namespace Service.Validators.Utils;

public class ValidationHelpers(IFlexibilityRepository flexibilityRepository, IVehicleSizeRepository vehicleSizeRepository)
{
    /// <summary>
    /// Determines whether a Flexibility with the specified ID exists.
    /// </summary>
    /// <param name="flexibilityId">The ID of the Flexibility to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the Flexibility exists; otherwise, false.</returns>
    public async Task<bool> FlexibilityIsValid(Guid flexibilityId, CancellationToken cancellationToken)
    {
        var result = await flexibilityRepository.GetByIdAsync(flexibilityId);
        return result != null;
    }

    /// <summary>
    /// Determines whether a VehicleSize with the specified ID exists.
    /// </summary>
    /// <param name="vehicleSizeId">The ID of the VehicleSize to validate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the VehicleSize exists; otherwise, false.</returns>
    public async Task<bool> VehicleSizeIsValid(Guid vehicleSizeId, CancellationToken cancellationToken)
    {
        var result = await vehicleSizeRepository.GetByIdAsync(vehicleSizeId);
        return result != null;
    }
}