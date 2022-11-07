using Valeting.Common.Messages;
using Valeting.Common.Exceptions;
using Valeting.Services.Interfaces;
using Valeting.Business.Flexibility;
using Valeting.Repositories.Interfaces;

namespace Valeting.Service
{
    public class FlexibilityService :IFlexibilityService
    {
        private readonly IFlexibilityRepository _flexibilityRepository;

        public FlexibilityService(IFlexibilityRepository flexibilityRepository)
        {
            _flexibilityRepository = flexibilityRepository ?? throw new Exception(string.Format(Messages.NotInitializeRepository, "Flexibility Repository"));
        }

        public async Task<FlexibilityDTO> FindByIDAsync(Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new InputException(Messages.InvalidFlexibilityId);

            var flexibilityDTO = await _flexibilityRepository.FindByIDAsync(id);
            if (flexibilityDTO == null)
                throw new NotFoundException(Messages.FlexibilityNotFound);

            return flexibilityDTO;
        }

        public async Task<FlexibilityListDTO> ListAllAsync(FlexibilityFilterDTO flexibilityFilterDTO)
        {
            if (flexibilityFilterDTO.PageNumber == 0)
                throw new InputException(Messages.InvalidPageNumber);

            return await _flexibilityRepository.ListAsync(flexibilityFilterDTO);
        }
    }
}
