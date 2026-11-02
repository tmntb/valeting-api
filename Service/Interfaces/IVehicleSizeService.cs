using Service.Models.VehicleSize;
using Service.Models.VehicleSize.Payload;

namespace Service.Interfaces;

public interface IVehicleSizeService
{
    /// <summary>
    /// Retrieves a vehicle size by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the vehicle size.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="VehicleSizeDto"/> corresponding to the specified ID.
    /// </returns>
    Task<VehicleSizeDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of vehicle sizes based on the specified filter parameters.
    /// </summary>
    /// <param name="vehicleSizeFilterDto">The filter parameters for pagination and optional criteria.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="VehicleSizePaginatedDtoResponse"/> 
    /// with the filtered vehicle sizes, total items, total pages, and pagination metadata.
    /// </returns>
    Task<VehicleSizePaginatedDtoResponse> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto, CancellationToken cancellationToken = default);
}