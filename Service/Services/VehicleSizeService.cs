using Common.Cache;
using Common.Cache.Interfaces;
using Common.Messages;
using Service.Interfaces;
using Service.Models.VehicleSize;
using Service.Models.VehicleSize.Payload;
using Service.Validators;
using Service.Validators.Utils;

namespace Service.Services;

public class VehicleSizeService(IVehicleSizeRepository vehicleSizeRepository, ICacheHandler cacheHandler) : IVehicleSizeService
{
    /// <inheritdoc />
    public async Task<VehicleSizeDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await cacheHandler.GetOrCreateRecordAsync(
            id,
            async () => await vehicleSizeRepository.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException(Messages.NotFound),
            new()
            {
                Id = id,
                AbsoluteExpireTime = TimeSpan.FromDays(1)
            }
        );
    }

    /// <inheritdoc />
    public async Task<VehicleSizePaginatedDtoResponse> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto, CancellationToken cancellationToken = default)
    {
        var paginatedVehicleSizeDtoResponse = new VehicleSizePaginatedDtoResponse();

        vehicleSizeFilterDto.ValidateRequest(new PaginatedVehicleSizeValidator());

        return await cacheHandler.GetOrCreateRecordAsync(
            vehicleSizeFilterDto,
            async () =>
            {
                var vehicleSizeDtoList = await vehicleSizeRepository.GetFilteredAsync(vehicleSizeFilterDto, cancellationToken);
                if (vehicleSizeDtoList.Count == 0)
                    throw new KeyNotFoundException(Messages.NotFound);

                paginatedVehicleSizeDtoResponse.TotalItems = vehicleSizeDtoList.Count();
                paginatedVehicleSizeDtoResponse.TotalPages = (int)Math.Ceiling((double)paginatedVehicleSizeDtoResponse.TotalItems / vehicleSizeFilterDto.PageSize);

                vehicleSizeDtoList = vehicleSizeDtoList
                    .OrderBy(x => x.Id)
                    .Skip((vehicleSizeFilterDto.PageNumber - 1) * vehicleSizeFilterDto.PageSize)
                    .Take(vehicleSizeFilterDto.PageSize)
                    .ToList();

                paginatedVehicleSizeDtoResponse.VehicleSizes = vehicleSizeDtoList;

                return paginatedVehicleSizeDtoResponse;
            },
            new()
            {
                ListType = CacheListType.VehicleSize,
                AbsoluteExpireTime = TimeSpan.FromMinutes(5)
            }
        );
    }
}