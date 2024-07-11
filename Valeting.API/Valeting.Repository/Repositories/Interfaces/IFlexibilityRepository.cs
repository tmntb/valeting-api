using Valeting.Repository.Models.Flexibility;

namespace Valeting.Repository.Repositories.Interfaces;

public interface IFlexibilityRepository
{
    Task<FlexibilityListDTO> GetAsync(FlexibilityFilterDTO flexibilityFilterDTO);
    Task<FlexibilityDTO> GetByIDAsync(Guid id);
}