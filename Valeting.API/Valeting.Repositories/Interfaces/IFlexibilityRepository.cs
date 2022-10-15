using Valeting.Business.Flexibility;

namespace Valeting.Repositories.Interfaces
{
    public interface IFlexibilityRepository
    {
        Task<FlexibilityDTO> FindByIDAsync(Guid id);
        Task<FlexibilityListDTO> ListAsync(FlexibilityFilterDTO flexibilityFilterDTO);
    }
}
