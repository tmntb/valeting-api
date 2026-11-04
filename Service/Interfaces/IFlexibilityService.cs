using Service.Models.Flexibility;
using Service.Models.Flexibility.Payload;

namespace Service.Interfaces;

public interface IFlexibilityService
{
    /// <summary>
    /// Retrieves a flexibility record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the flexibility record.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>The corresponding <see cref="FlexibilityDto"/> object.</returns>
    Task<FlexibilityDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated and optionally filtered list of flexibilities.
    /// </summary>
    /// <param name="flexibilityFilterDto">The filter and pagination parameters for the request.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>
    /// A <see cref="FlexibilityPaginatedDtoResponse"/> object containing a list of flexibilities, along with pagination details such as total items 
    /// and total pages.
    /// </returns>
    Task<FlexibilityPaginatedDtoResponse> GetFilteredAsync(FlexibilityFilterDto flexibilityFilterDto, CancellationToken cancellationToken = default);
}