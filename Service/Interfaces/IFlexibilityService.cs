using Common.Models.Flexibility;

namespace Service.Interfaces;

public interface IFlexibilityService
{
    Task<FlexibilityDto> GetByIdAsync(Guid id);
    Task<FlexibilityPaginatedDtoResponse> GetFilteredAsync(FlexibilityFilterDto flexibilityFilterDto);
}