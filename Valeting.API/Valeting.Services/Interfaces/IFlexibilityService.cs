using Valeting.Services.Objects.Flexibility;

namespace Valeting.Services.Interfaces;

public interface IFlexibilityService
{
    Task<GetFlexibilitySVResponse> GetAsync(GetFlexibilitySVRequest getFlexibilitySVRequest);
    Task<PaginatedFlexibilitySVResponse> ListAllAsync(PaginatedFlexibilitySVRequest paginatedFlexibilitySVRequest);
}