using FluentValidation;
using Valeting.Common.Cache;
using Valeting.Core.Interfaces;
using Valeting.Common.Messages;
using Valeting.Services.Validators;
using Valeting.Repository.Interfaces;
using Valeting.Common.Cache.Interfaces;
using Valeting.Common.Models.Flexibility;

namespace Valeting.Core.Services;

public class FlexibilityService(IFlexibilityRepository flexibilityRepository, ICacheHandler cacheHandler) : IFlexibilityService
{
    public async Task<PaginatedFlexibilityDtoResponse> GetFilteredAsync(PaginatedFlexibilityDtoRequest paginatedFlexibilityDtoRequest)
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
                var flexibilityDtoList = await flexibilityRepository.GetFilteredAsync(paginatedFlexibilityDtoRequest.Filter);
                if(flexibilityDtoList.Count == 0 )
                    throw new KeyNotFoundException(Messages.FlexibilityNotFound);

                paginatedFlexibilityDtoResponse.TotalItems = flexibilityDtoList.Count();
                var nrPages = decimal.Divide(paginatedFlexibilityDtoResponse.TotalItems, paginatedFlexibilityDtoRequest.Filter.PageSize);
                paginatedFlexibilityDtoResponse.TotalPages = (int)(nrPages - Math.Truncate(nrPages) > 0 ? Math.Truncate(nrPages) + 1 : Math.Truncate(nrPages));

                flexibilityDtoList = flexibilityDtoList.OrderBy(x => x.Id).ToList();
                flexibilityDtoList = flexibilityDtoList.Skip((paginatedFlexibilityDtoRequest.Filter.PageNumber - 1) * paginatedFlexibilityDtoRequest.Filter.PageSize).Take(paginatedFlexibilityDtoRequest.Filter.PageSize).ToList();

                paginatedFlexibilityDtoResponse.Flexibilities = flexibilityDtoList;
                return paginatedFlexibilityDtoResponse;
            },
            new CacheOptions
            {
                ListType = CacheListType.Flexibility,
                AbsoluteExpireTime = TimeSpan.FromDays(5)
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
                getFlexibilityDtoResponse.Flexibility = await flexibilityRepository.GetByIdAsync(getFlexibilityDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.FlexibilityNotFound);
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