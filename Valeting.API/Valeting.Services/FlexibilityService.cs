using Valeting.Services.Interfaces;
using Valeting.Business.Flexibility;
using Valeting.Repositories.Interfaces;
using System.Reflection;

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
                throw new Exception("Flexibility not found");

            return flexibilityDTO;
        }

        public async Task<FlexibilityListDTO> ListAllAsync(FlexibilityFilterDTO flexibilityFilterDTO)
        {
            if (flexibilityFilterDTO.PageNumber == 0)
                throw new Exception("pageNumber é 0");

            return await _flexibilityRepository.ListAsync(flexibilityFilterDTO);
        }
    }
}
