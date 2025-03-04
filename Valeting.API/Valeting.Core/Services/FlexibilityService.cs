using AutoMapper;
using FluentValidation;
using Valeting.Common.Cache;
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
            throw new ValidationException(result.Errors);
        }

        return await cacheHandler.GetOrCreateRecordAsync(
            paginatedFlexibilityDtoRequest.Filter,
            async () =>
            {
                var flexibilityListDto = await flexibilityRepository.GetAsync(paginatedFlexibilityDtoRequest.Filter) ?? throw new KeyNotFoundException(Messages.FlexibilityNotFound);
                return mapper.Map<PaginatedFlexibilityDtoResponse>(flexibilityListDto);
            },
            new CacheOptions
            {
                ListType = CacheListType.Flexibility,
                AbsoluteExpireTime = TimeSpan.FromMinutes(5)
            }
        );
    }

    public async Task<GetFlexibilityDtoResponse> GetByIdAsync(GetFlexibilityDtoRequest getFlexibilityDtoRequest)
    {
        var getFlexibilityDtoResponse = new GetFlexibilityDtoResponse();

        var validator = new GetFlexibilityValidator();
        var result = validator.Validate(getFlexibilityDtoRequest);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        return await cacheHandler.GetOrCreateRecordAsync(
            getFlexibilityDtoRequest,
            async () =>
            {
                var flexibilityDto = await flexibilityRepository.GetByIdAsync(getFlexibilityDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.FlexibilityNotFound);
                getFlexibilityDtoResponse.Flexibility = mapper.Map<FlexibilityDto>(flexibilityDto);
                return getFlexibilityDtoResponse;
            },
             new CacheOptions
             {
                 Id = getFlexibilityDtoRequest.Id,
                 AbsoluteExpireTime = TimeSpan.FromDays(1)
             }
        );
    }
}