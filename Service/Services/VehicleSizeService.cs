using Common.Cache;
using Common.Cache.Interfaces;
using Common.Messages;
using Common.Models.VehicleSize;
using Service.Interfaces;
using Service.Validators;
using Service.Validators.Utils;

namespace Service.Services;

public class VehicleSizeService(IVehicleSizeRepository vehicleSizeRepository, ICacheHandler cacheHandler) : IVehicleSizeService
{
    /// <inheritdoc />
    public async Task<VehicleSizeDto> GetByIdAsync(Guid id)
    {
        var getVehicleSizeDtoResponse = new GetVehicleSizeDtoResponse();

        return await cacheHandler.GetOrCreateRecordAsync(
            id,
            async () =>
            {
                return await vehicleSizeRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException(Messages.NotFound);
            },
            new()
            {
                Id = id,
                AbsoluteExpireTime = TimeSpan.FromDays(1)
            }
        );
    }

    /// <inheritdoc />
    public async Task<VehicleSizePaginatedDtoResponse> GetFilteredAsync(VehicleSizeFilterDto vehicleSizeFilterDto)
    {
        var paginatedVehicleSizeDtoResponse = new VehicleSizePaginatedDtoResponse();

        vehicleSizeFilterDto.ValidateRequest(new PaginatedVehicleSizeValidator());

        return await cacheHandler.GetOrCreateRecordAsync(
            vehicleSizeFilterDto,
            async () =>
            {
                var vehicleSizeDtoList = await vehicleSizeRepository.GetFilteredAsync(vehicleSizeFilterDto);
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