using Valeting.Business;

namespace Valeting.Services.Interfaces
{
    public interface IFlexibilityService
    {
        Task<FlexibilityDTO> FindByIDAsync(Guid id);
        Task<IEnumerable<FlexibilityDTO>> ListAllAsync();
    }
}

