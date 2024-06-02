using Valeting.Services.Objects.Flexibility;

namespace Valeting.Services.Interfaces;

public interface IFlexibilityService
{
    Task<PaginatedFlexibilitySVResponse> GetAsync(PaginatedFlexibilitySVRequest paginatedFlexibilitySVRequest);
    Task<GetFlexibilitySVResponse> GetByIdAsync(GetFlexibilitySVRequest getFlexibilitySVRequest);
}