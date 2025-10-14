using Common.Models.Flexibility;

namespace Service.Interfaces;

public interface IFlexibilityRepository
{
    Task<List<FlexibilityDto>> GetFilteredAsync(FlexibilityFilterDto flexibilityFilterDto);
    Task<FlexibilityDto> GetByIdAsync(Guid id);
}