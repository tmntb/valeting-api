using Common.Cache;
using Common.Cache.Interfaces;
using Common.Messages;
using Service.Interfaces;
using Service.Models.Flexibility;
using Service.Models.Flexibility.Payload;
using Service.Validators;
using Service.Validators.Utils;

namespace Service.Services;

public class FlexibilityService(IFlexibilityRepository flexibilityRepository, ICacheHandler cacheHandler) : IFlexibilityService
{
    /// <inheritdoc />
    public async Task<FlexibilityDto> GetByIdAsync(Guid id)
    {
        return await cacheHandler.GetOrCreateRecordAsync(
            id,
            async () =>
            {
                return await flexibilityRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException(Messages.NotFound);
            },
            new()
            {
                Id = id,
                AbsoluteExpireTime = TimeSpan.FromDays(1)
            }
        );
    }

    /// <inheritdoc />
    public async Task<FlexibilityPaginatedDtoResponse> GetFilteredAsync(FlexibilityFilterDto flexibilityFilterDto)
    {
        var paginatedFlexibilityDtoResponse = new FlexibilityPaginatedDtoResponse();

        flexibilityFilterDto.ValidateRequest(new PaginatedFlexibilityValidator());

        return await cacheHandler.GetOrCreateRecordAsync(
            flexibilityFilterDto,
            async () =>
            {
                var flexibilityDtoList = await flexibilityRepository.GetFilteredAsync(flexibilityFilterDto);
                if (flexibilityDtoList.Count == 0)
                    throw new KeyNotFoundException(Messages.NotFound);

                paginatedFlexibilityDtoResponse.TotalItems = flexibilityDtoList.Count();
                paginatedFlexibilityDtoResponse.TotalPages = (int)Math.Ceiling((double)paginatedFlexibilityDtoResponse.TotalItems / flexibilityFilterDto.PageSize);

                flexibilityDtoList = flexibilityDtoList
                    .OrderBy(x => x.Id)
                    .Skip((flexibilityFilterDto.PageNumber - 1) * flexibilityFilterDto.PageSize)
                    .Take(flexibilityFilterDto.PageSize)
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
}