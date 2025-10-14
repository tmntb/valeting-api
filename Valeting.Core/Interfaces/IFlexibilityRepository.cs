using Valeting.Common.Models.Flexibility;

namespace Valeting.Core.Interfaces;

public interface IFlexibilityRepository
{
    Task<List<FlexibilityDto>> GetFilteredAsync(FlexibilityFilterDto flexibilityFilterDto);
    Task<FlexibilityDto> GetByIdAsync(Guid id);
}