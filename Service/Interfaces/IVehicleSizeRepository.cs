using Service.Models.VehicleSize;
using Service.Models.VehicleSize.Payload;

namespace Service.Interfaces;

public interface IVehicleSizeRepository
{
    /// <summary>
    /// Retrieves a filtered list of vehicle sizes from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the vehicle size to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token from the task.</param>
    /// <returns>A task that returns a list of <see cref="VehicleSizeDto"/> matching the filter criteria.</returns>
    Task<VehicleSizeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single vehicle size by its unique identifier.
    /// </summary>
    /// <param name="vehicleSizeFilterDto">The filter criteria, including optional active status.</param>
    /// <param name="cancellationToken">The cancellation token from the task.</param>
    /// <returns>A task that returns the <see cref="VehicleSizeDto"/> if found; otherwise, null.</returns>
    Task<List<VehicleSizeDto>> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto, CancellationToken cancellationToken = default);
}