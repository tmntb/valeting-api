using Common.Models.Flexibility;

namespace Service.Interfaces;

public interface IFlexibilityRepository
{
    Task<FlexibilityDto> GetByIdAsync(Guid id);
    Task<List<FlexibilityDto>> GetFilteredAsync(FlexibilityFilterDto flexibilityFilterDto);
}