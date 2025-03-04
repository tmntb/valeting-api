using Valeting.Common.Models.Flexibility;

namespace Valeting.Repository.Interfaces;

public interface IFlexibilityRepository
{
    Task<FlexibilityListDto> GetAsync(FlexibilityFilterDto flexibilityFilterDto);
    Task<FlexibilityDto> GetByIdAsync(Guid id);
}