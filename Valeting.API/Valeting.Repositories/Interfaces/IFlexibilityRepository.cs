using Valeting.Business;

namespace Valeting.Repositories.Interfaces
{
    public interface IFlexibilityRepository
    {
        Task<FlexibilityDTO> FindByIDAsync(Guid id);
        Task<IEnumerable<FlexibilityDTO>> ListAsync();
    }
}
