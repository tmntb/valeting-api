using Valeting.Business.Flexibility;

namespace Valeting.Repositories.Interfaces;

public interface IFlexibilityRepository
{
    Task<FlexibilityListDTO> GetAsync(FlexibilityFilterDTO flexibilityFilterDTO);
    Task<FlexibilityDTO> GetByIDAsync(Guid id);
}