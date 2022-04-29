using DJValeting.Business;
using DJValeting.Repositories.Interfaces;

namespace DJValeting.Service
{
    public class FlexibilityService
    {
        private IFlexibilityRepository _flexibilityRepository;

        public FlexibilityService(IFlexibilityRepository flexibilityRepository)
        {
            _flexibilityRepository = flexibilityRepository ?? throw new Exception("flexibilityRepository is null");
        }

        public async Task<FlexibilityDTO> FindByIDAsync(Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new Exception("FlexibilityId is empty");

            FlexibilityDTO flexibilityDTO = await _flexibilityRepository.FindByIDAsync(id);
            if (flexibilityDTO == null)
                throw new Exception("Booking not found");

            return flexibilityDTO;
        }

        public async Task<IEnumerable<FlexibilityDTO>> ListAllAsync()
        {
            IEnumerable<FlexibilityDTO> flexibilityDTODTOs = await _flexibilityRepository.ListAsync();
            if (flexibilityDTODTOs == null || !flexibilityDTODTOs.Any())
                throw new Exception("None flexibility was found");

            return flexibilityDTODTOs;
        }
    }
}
