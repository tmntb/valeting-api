using Valeting.Common.Cache;
using Valeting.Common.Cache.Interfaces;
using Valeting.Common.Messages;
using Valeting.Common.Models.Flexibility;
using Valeting.Core.Interfaces;
using Valeting.Core.Validators;
using Valeting.Core.Validators.Utils;
using Valeting.Repository.Interfaces;

namespace Valeting.Core.Services;

public class FlexibilityService(IFlexibilityRepository flexibilityRepository, ICacheHandler cacheHandler) : IFlexibilityService
{
    public async Task<PaginatedFlexibilityDtoResponse> GetFilteredAsync(PaginatedFlexibilityDtoRequest paginatedFlexibilityDtoRequest)
    {
        var paginatedFlexibilityDtoResponse = new PaginatedFlexibilityDtoResponse();

        paginatedFlexibilityDtoRequest.ValidateRequest(new PaginatedFlexibilityValidator());

        return await cacheHandler.GetOrCreateRecordAsync(
            paginatedFlexibilityDtoRequest.Filter,
            async () =>
            {
                var flexibilityDtoList = await flexibilityRepository.GetFilteredAsync(paginatedFlexibilityDtoRequest.Filter);
                if (flexibilityDtoList.Count == 0)
                    throw new KeyNotFoundException(Messages.FlexibilityNotFound);

                paginatedFlexibilityDtoResponse.TotalItems = flexibilityDtoList.Count();
                var nrPages = decimal.Divide(paginatedFlexibilityDtoResponse.TotalItems, paginatedFlexibilityDtoRequest.Filter.PageSize);
                paginatedFlexibilityDtoResponse.TotalPages = (int)(nrPages - Math.Truncate(nrPages) > 0 ? Math.Truncate(nrPages) + 1 : Math.Truncate(nrPages));

                flexibilityDtoList = flexibilityDtoList.OrderBy(x => x.Id).ToList();
                flexibilityDtoList = flexibilityDtoList.Skip((paginatedFlexibilityDtoRequest.Filter.PageNumber - 1) * paginatedFlexibilityDtoRequest.Filter.PageSize).Take(paginatedFlexibilityDtoRequest.Filter.PageSize).ToList();

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
                getFlexibilityDtoResponse.Flexibility = await flexibilityRepository.GetByIdAsync(getFlexibilityDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.FlexibilityNotFound);
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