using Common.Models.Flexibility;

namespace Service.Interfaces;

public interface IFlexibilityService
{
    Task<PaginatedFlexibilityDtoResponse> GetFilteredAsync(PaginatedFlexibilityDtoRequest paginatedFlexibilityDtoRequest);
    Task<GetFlexibilityDtoResponse> GetByIdAsync(GetFlexibilityDtoRequest getFlexibilityDtoRequest);
}