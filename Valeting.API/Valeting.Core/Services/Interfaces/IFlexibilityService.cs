using Valeting.Core.Models.Flexibility;

namespace Valeting.Core.Services.Interfaces;

public interface IFlexibilityService
{
    Task<PaginatedFlexibilitySVResponse> GetAsync(PaginatedFlexibilitySVRequest paginatedFlexibilitySVRequest);
    Task<GetFlexibilitySVResponse> GetByIdAsync(GetFlexibilitySVRequest getFlexibilitySVRequest);
}