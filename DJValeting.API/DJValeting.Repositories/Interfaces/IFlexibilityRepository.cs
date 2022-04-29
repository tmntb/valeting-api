using DJValeting.Business;

namespace DJValeting.Repositories.Interfaces
{
    public interface IFlexibilityRepository
    {
        Task<FlexibilityDTO> FindByIDAsync(Guid id);
        Task<IEnumerable<FlexibilityDTO>> ListAsync();
    }
}
