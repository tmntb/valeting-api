using Valeting.Business;
using Valeting.Services.Interfaces;
using Valeting.Repositories.Interfaces;

namespace Valeting.Service
{
    public class FlexibilityService :IFlexibilityService
    {
        private readonly IFlexibilityRepository _flexibilityRepository;

        public FlexibilityService(IFlexibilityRepository flexibilityRepository)
        {
            _flexibilityRepository = flexibilityRepository ?? throw new Exception("flexibilityRepository is null");
        }

        public async Task<FlexibilityDTO> FindByIDAsync(Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new Exception("FlexibilityId is empty");

            var flexibilityDTO = await _flexibilityRepository.FindByIDAsync(id);
            if (flexibilityDTO == null)
                throw new Exception("Booking not found");

            return flexibilityDTO;
        }

        public async Task<IEnumerable<FlexibilityDTO>> ListAllAsync()
        {
            var flexibilityDTODTOs = await _flexibilityRepository.ListAsync();
            if (flexibilityDTODTOs == null || !flexibilityDTODTOs.Any())
                throw new Exception("None flexibility was found");

            return flexibilityDTODTOs;
        }
    }
}
