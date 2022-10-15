using Valeting.Business.Flexibility;

namespace Valeting.Services.Interfaces
{
    public interface IFlexibilityService
    {
        Task<FlexibilityDTO> FindByIDAsync(Guid id);
        Task<FlexibilityListDTO> ListAllAsync(FlexibilityFilterDTO flexibilityFilterDTO);
    }
}

