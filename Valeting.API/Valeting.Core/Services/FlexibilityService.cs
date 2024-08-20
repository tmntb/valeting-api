﻿using AutoMapper;

using System.Net;

using Valeting.Common.Messages;
using Valeting.Core.Validators;
using Valeting.Core.Models.Flexibility;
using Valeting.Core.Services.Interfaces;
using Valeting.Repository.Models.Flexibility;
using Valeting.Repository.Repositories.Interfaces;

namespace Valeting.Core.Services;

public class FlexibilityService(IFlexibilityRepository flexibilityRepository, IMapper mapper) :IFlexibilityService
{
    public async Task<PaginatedFlexibilitySVResponse> GetAsync(PaginatedFlexibilitySVRequest paginatedFlexibilitySVRequest)
    {
        var paginatedFlexibilitySVResponse = new PaginatedFlexibilitySVResponse();

        var validator = new PaginatedFlexibilityValidator();
        var result = validator.Validate(paginatedFlexibilitySVRequest);
        if (!result.IsValid)
        {
            paginatedFlexibilitySVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return paginatedFlexibilitySVResponse;
        }

        var flexibilityFilterDTO = mapper.Map<FlexibilityFilterDTO>(paginatedFlexibilitySVRequest.Filter);

        var flexibilityListDTO = await flexibilityRepository.GetAsync(flexibilityFilterDTO);
        if (flexibilityListDTO == null)
        {
            paginatedFlexibilitySVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.FlexibilityNotFound
            };
            return paginatedFlexibilitySVResponse;
        }

        return mapper.Map<PaginatedFlexibilitySVResponse>(flexibilityListDTO);
    }

    public async Task<GetFlexibilitySVResponse> GetByIdAsync(GetFlexibilitySVRequest getFlexibilitySVRequest)
    {
        var getFlexibilitySVResponse = new GetFlexibilitySVResponse();

        var validator = new GetFlexibilityValidator();
        var result = validator.Validate(getFlexibilitySVRequest);
        if (!result.IsValid)
        {
            getFlexibilitySVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return getFlexibilitySVResponse;
        }

        var flexibilityDTO = await flexibilityRepository.GetByIdAsync(getFlexibilitySVRequest.Id);
        if (flexibilityDTO == null)
        {
            getFlexibilitySVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.FlexibilityNotFound
            };
            return getFlexibilitySVResponse;
        }
        
        getFlexibilitySVResponse.Flexibility = mapper.Map<FlexibilitySV>(flexibilityDTO);
        return getFlexibilitySVResponse;
    }
}