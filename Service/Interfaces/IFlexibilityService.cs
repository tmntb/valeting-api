using Service.Models.Flexibility;
using Service.Models.Flexibility.Payload;

namespace Service.Interfaces;

public interface IFlexibilityService
{
    /// <summary>
    /// Retrieves a flexibility record by its unique identifier.
    /// </summary>
    /// <remarks>
    /// Attempts to retrieve the flexibility data from cache first.  
    /// If not found, it fetches the record from the repository, caches it for one day and returns the result.  
    /// </remarks>
    /// <param name="id">The unique identifier of the flexibility record.</param>
    /// <returns>The corresponding <see cref="FlexibilityDto"/> object.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no flexibility is found with the given ID.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs during retrieval or caching.</exception>
    Task<FlexibilityDto> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a paginated and optionally filtered list of flexibilities.
    /// </summary>
    /// <remarks>
    /// Validates the provided filter request and attempts to retrieve cached data if available.  
    /// If not found in cache, it queries the repository for matching records, applies pagination and ordering, caches the results for five days,  
    /// and returns the paginated response.
    /// </remarks>
    /// <param name="flexibilityFilterDto">The filter and pagination parameters for the request.</param>
    /// <returns>
    /// A <see cref="FlexibilityPaginatedDtoResponse"/> object containing a list of flexibilities, along with pagination details such as total items 
    /// and total pages.
    /// </returns>
    /// <exception cref="ValidationException">Thrown when the provided filter parameters are invalid.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no flexibilities match the given filters.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs during retrieval or caching.</exception>
    Task<FlexibilityPaginatedDtoResponse> GetFilteredAsync(FlexibilityFilterDto flexibilityFilterDto);
}