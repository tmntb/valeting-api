using System.Net;

using Valeting.Common.Messages;
using Valeting.Services.Interfaces;
using Valeting.Services.Validators;
using Valeting.Business.Flexibility;
using Valeting.Repositories.Interfaces;
using Valeting.Services.Objects.Flexibility;

namespace Valeting.Services;

public class FlexibilityService(IFlexibilityRepository flexibilityRepository) :IFlexibilityService
{
    public async Task<GetFlexibilitySVResponse> GetAsync(GetFlexibilitySVRequest getFlexibilitySVRequest)
    {
        var getFlexibilitySVResponse = new GetFlexibilitySVResponse();

        var validator = new GetFlexibilityValidator();
        var result = validator.Validate(getFlexibilitySVRequest);
        if(!result.IsValid)
        {
            getFlexibilitySVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return getFlexibilitySVResponse;
        }


        var flexibilityDTO = await flexibilityRepository.FindByIDAsync(getFlexibilitySVRequest.Id);
        if (flexibilityDTO == null)
        {
            getFlexibilitySVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.FlexibilityNotFound
            };
            return getFlexibilitySVResponse;
        }

        getFlexibilitySVResponse.Id = flexibilityDTO.Id;
        getFlexibilitySVResponse.Description = flexibilityDTO.Description;
        getFlexibilitySVResponse.Active = flexibilityDTO.Active;
        return getFlexibilitySVResponse;
    }

    public async Task<PaginatedFlexibilitySVResponse> ListAllAsync(PaginatedFlexibilitySVRequest paginatedFlexibilitySVRequest)
    {
        var paginatedFlexibilitySVResponse = new PaginatedFlexibilitySVResponse();

        var validator = new PaginatedFlexibilityValidator();
        var result = validator.Validate(paginatedFlexibilitySVRequest);
        if(!result.IsValid)
        {
            paginatedFlexibilitySVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return paginatedFlexibilitySVResponse;
        }
        
        var flexibilityFilterDTO = new FlexibilityFilterDTO()
        {
            PageNumber = paginatedFlexibilitySVRequest.Filter.PageNumber,
            PageSize = paginatedFlexibilitySVRequest.Filter.PageSize
        };

        var flexibilityListDTO =  await flexibilityRepository.ListAsync(flexibilityFilterDTO);
        if(flexibilityListDTO == null)
        {
            paginatedFlexibilitySVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.FlexibilityNotFound
            };
            return paginatedFlexibilitySVResponse;
        }

        paginatedFlexibilitySVResponse.TotalItems = flexibilityListDTO.TotalItems;
        paginatedFlexibilitySVResponse.TotalPages = flexibilityListDTO.TotalPages;

        paginatedFlexibilitySVResponse.Flexibilities = flexibilityListDTO.Flexibilities.Select(x => 
            new FlexibilitySV()
            {
                Id = x.Id,
                Description = x.Description,
                Active = x.Active
            }
        ).ToList();

        return paginatedFlexibilitySVResponse;
    }
}