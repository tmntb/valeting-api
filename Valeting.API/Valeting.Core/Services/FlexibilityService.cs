using AutoMapper;
using System.Net;
using Valeting.Core.Interfaces;
using Valeting.Common.Messages;
using Valeting.Services.Validators;
using Valeting.Repository.Interfaces;
using Valeting.Common.Cache.Interfaces;
using Valeting.Common.Models.Flexibility;

namespace Valeting.Core.Services;

public class FlexibilityService(IFlexibilityRepository flexibilityRepository, ICacheHandler cacheHandler, IMapper mapper) : IFlexibilityService
{
    public async Task<PaginatedFlexibilityDtoResponse> GetAsync(PaginatedFlexibilityDtoRequest paginatedFlexibilityDtoRequest)
    {
        var paginatedFlexibilityDtoResponse = new PaginatedFlexibilityDtoResponse();

        var validator = new PaginatedFlexibilityValidator();
        var result = validator.Validate(paginatedFlexibilityDtoRequest);
        if (!result.IsValid)
        {
            paginatedFlexibilityDtoResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return paginatedFlexibilityDtoResponse;
        }

        var recordKey = string.Format("ListFlexibility_{0}_{1}_{2}", paginatedFlexibilityDtoRequest.Filter.PageNumber, paginatedFlexibilityDtoRequest.Filter.PageSize, paginatedFlexibilityDtoRequest.Filter.Active);
        paginatedFlexibilityDtoResponse = cacheHandler.GetRecord<PaginatedFlexibilityDtoResponse>(recordKey);
        if (paginatedFlexibilityDtoResponse == null)
        {
            var flexibilityFilterDto = mapper.Map<FlexibilityFilterDto>(paginatedFlexibilityDtoRequest.Filter);

            var flexibilityListDto = await flexibilityRepository.GetAsync(flexibilityFilterDto);
            if (flexibilityListDto == null)
            {
                paginatedFlexibilityDtoResponse.Error = new()
                {
                    ErrorCode = (int)HttpStatusCode.NotFound,
                    Message = Messages.FlexibilityNotFound
                };
                return paginatedFlexibilityDtoResponse;
            }

            paginatedFlexibilityDtoResponse = mapper.Map<PaginatedFlexibilityDtoResponse>(flexibilityListDto);

            cacheHandler.SetRecord(recordKey, paginatedFlexibilityDtoResponse, TimeSpan.FromMinutes(5));
        }

        return paginatedFlexibilityDtoResponse;
    }

    public async Task<GetFlexibilityDtoResponse> GetByIdAsync(GetFlexibilityDtoRequest getFlexibilityDtoRequest)
    {
        var getFlexibilityDtoResponse = new GetFlexibilityDtoResponse();

        var validator = new GetFlexibilityValidator();
        var result = validator.Validate(getFlexibilityDtoRequest);
        if (!result.IsValid)
        {
            getFlexibilityDtoResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return getFlexibilityDtoResponse;
        }

        var recordKey = string.Format("Flexibility_{0}", getFlexibilityDtoRequest.Id);
        getFlexibilityDtoResponse = cacheHandler.GetRecord<GetFlexibilityDtoResponse>(recordKey);
        if (getFlexibilityDtoResponse == null)
        {

            var flexibilityDto = await flexibilityRepository.GetByIdAsync(getFlexibilityDtoRequest.Id);
            if (flexibilityDto == null)
            {
                getFlexibilityDtoResponse.Error = new()
                {
                    ErrorCode = (int)HttpStatusCode.NotFound,
                    Message = Messages.FlexibilityNotFound
                };
                return getFlexibilityDtoResponse;
            }

            getFlexibilityDtoResponse.Flexibility = mapper.Map<FlexibilityDto>(flexibilityDto);

            cacheHandler.SetRecord(recordKey, getFlexibilityDtoResponse, TimeSpan.FromDays(1));
        }

        return getFlexibilityDtoResponse;
    }
}