using Valeting.Common.Messages;
using Valeting.Common.Exceptions;
using Valeting.Services.Interfaces;
using Valeting.Business.Flexibility;
using Valeting.Repositories.Interfaces;

namespace Valeting.Services;

public class FlexibilityService(IFlexibilityRepository flexibilityRepository) :IFlexibilityService
{
    public async Task<FlexibilityDTO> FindByIDAsync(Guid id)
    {
        if (id.Equals(Guid.Empty))
            throw new InputException(Messages.InvalidFlexibilityId);

        var flexibilityDTO = await flexibilityRepository.FindByIDAsync(id);
        if (flexibilityDTO == null)
            throw new NotFoundException(Messages.FlexibilityNotFound);

        return flexibilityDTO;
    }

    public async Task<FlexibilityListDTO> ListAllAsync(FlexibilityFilterDTO flexibilityFilterDTO)
    {
        if (flexibilityFilterDTO.PageNumber == 0)
            throw new InputException(Messages.InvalidPageNumber);

        return await flexibilityRepository.ListAsync(flexibilityFilterDTO);
    }
}