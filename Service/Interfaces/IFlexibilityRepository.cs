using Service.Models.Flexibility;
using Service.Models.Flexibility.Payload;

namespace Service.Interfaces;

public interface IFlexibilityRepository
{
    /// <summary>
    /// Retrieves a flexibility record by its unique identifier from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the flexibility.</param>
    /// <returns>A task that returns a <see cref="FlexibilityDto"/> if found; otherwise, null.</returns>
    Task<FlexibilityDto> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a filtered list of flexibilities from the database based on the specified filter parameters.
    /// </summary>
    /// <param name="flexibilityFilterDto">The filter parameters including optional active status.</param>
    /// <returns>A task that returns a list of <see cref="FlexibilityDto"/> matching the filter criteria.</returns>
    Task<List<FlexibilityDto>> GetFilteredAsync(FlexibilityFilterDto flexibilityFilterDto);
}