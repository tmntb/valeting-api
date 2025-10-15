using Service.Models.Flexibility;
using Service.Models.Flexibility.Payload;

namespace Service.Interfaces;

public interface IFlexibilityRepository
{
    Task<FlexibilityDto> GetByIdAsync(Guid id);
    Task<List<FlexibilityDto>> GetFilteredAsync(FlexibilityFilterDto flexibilityFilterDto);
}