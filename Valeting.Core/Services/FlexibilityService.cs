using Valeting.Common.Cache;
using Valeting.Common.Cache.Interfaces;
using Valeting.Common.Messages;
using Valeting.Common.Models.Flexibility;
using Valeting.Core.Interfaces;
using Valeting.Core.Validators;
using Valeting.Core.Validators.Utils;

namespace Valeting.Core.Services;

public class FlexibilityService(IFlexibilityRepository flexibilityRepository, ICacheHandler cacheHandler) : IFlexibilityService
{
    public async Task<PaginatedFlexibilityDtoResponse> GetFilteredAsync(PaginatedFlexibilityDtoRequest paginatedFlexibilityDtoRequest)
    {
        var paginatedFlexibilityDtoResponse = new PaginatedFlexibilityDtoResponse();

        paginatedFlexibilityDtoRequest.ValidateRequest(new PaginatedFlexibilityValidator());

        return await cacheHandler.GetOrCreateRecordAsync(
            paginatedFlexibilityDtoRequest,
            async () =>
            {
                var flexibilityDtoList = await flexibilityRepository.GetFilteredAsync(paginatedFlexibilityDtoRequest.Filter);
                if (flexibilityDtoList.Count == 0)
                    throw new KeyNotFoundException(Messages.NotFound);

                paginatedFlexibilityDtoResponse.TotalItems = flexibilityDtoList.Count();
                paginatedFlexibilityDtoResponse.TotalPages = (int)Math.Ceiling((double)paginatedFlexibilityDtoResponse.TotalItems / paginatedFlexibilityDtoRequest.Filter.PageSize);

                flexibilityDtoList = flexibilityDtoList
                    .OrderBy(x => x.Id)
                    .Skip((paginatedFlexibilityDtoRequest.Filter.PageNumber - 1) * paginatedFlexibilityDtoRequest.Filter.PageSize)
                    .Take(paginatedFlexibilityDtoRequest.Filter.PageSize)
                    .ToList();

                paginatedFlexibilityDtoResponse.Flexibilities = flexibilityDtoList;
                return paginatedFlexibilityDtoResponse;
            },
            new()
            {
                ListType = CacheListType.Flexibility,
                AbsoluteExpireTime = TimeSpan.FromDays(5)
            }
        );
    }

    public async Task<GetFlexibilityDtoResponse> GetByIdAsync(GetFlexibilityDtoRequest getFlexibilityDtoRequest)
    {
        var getFlexibilityDtoResponse = new GetFlexibilityDtoResponse();

        getFlexibilityDtoRequest.ValidateRequest(new GetFlexibilityValidator());

        return await cacheHandler.GetOrCreateRecordAsync(
            getFlexibilityDtoRequest,
            async () =>
            {
                getFlexibilityDtoResponse.Flexibility = await flexibilityRepository.GetByIdAsync(getFlexibilityDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.NotFound);
                return getFlexibilityDtoResponse;
            },
            new()
            {
                Id = getFlexibilityDtoRequest.Id,
                AbsoluteExpireTime = TimeSpan.FromDays(1)
            }
        );
    }
}