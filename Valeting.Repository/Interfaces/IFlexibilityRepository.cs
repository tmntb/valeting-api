using Valeting.Common.Models.Flexibility;

namespace Valeting.Repository.Interfaces;

public interface IFlexibilityRepository
{
    Task<List<FlexibilityDto>> GetFilteredAsync(FlexibilityFilterDto flexibilityFilterDto);
    Task<FlexibilityDto> GetByIdAsync(Guid id);
}