using FluentValidation;
using Valeting.Common.Cache;
using Valeting.Core.Interfaces;
using Valeting.Common.Messages;
using Valeting.Services.Validators;
using Valeting.Repository.Interfaces;
using Valeting.Common.Cache.Interfaces;
using Valeting.Common.Models.VehicleSize;

namespace Valeting.Core.Services;

public class VehicleSizeService(IVehicleSizeRepository vehicleSizeRepository, ICacheHandler cacheHandler) : IVehicleSizeService
{
    public async Task<PaginatedVehicleSizeDtoResponse> GetFilteredAsync(PaginatedVehicleSizeDtoRequest paginatedVehicleSizeDtoRequest)
    {
        var paginatedVehicleSizeDtoResponse = new PaginatedVehicleSizeDtoResponse();

        var validator = new PaginatedVehicleSizeValidator();
        var result = validator.Validate(paginatedVehicleSizeDtoRequest);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        return await cacheHandler.GetOrCreateRecordAsync(
            paginatedVehicleSizeDtoRequest,
            async () =>
            {
                var vehicleSizeDtoList = await vehicleSizeRepository.GetFilteredAsync(paginatedVehicleSizeDtoRequest.Filter);
                if (vehicleSizeDtoList.Count == 0)
                    throw new KeyNotFoundException(Messages.VehicleSizeNotFound);

                paginatedVehicleSizeDtoResponse.TotalItems = vehicleSizeDtoList.Count();
                var nrPages = decimal.Divide(paginatedVehicleSizeDtoResponse.TotalItems, paginatedVehicleSizeDtoRequest.Filter.PageSize);
                paginatedVehicleSizeDtoResponse.TotalPages = (int)(nrPages - Math.Truncate(nrPages) > 0 ? Math.Truncate(nrPages) + 1 : Math.Truncate(nrPages));

                vehicleSizeDtoList = vehicleSizeDtoList.OrderBy(x => x.Id).ToList();
                vehicleSizeDtoList = vehicleSizeDtoList.Skip((paginatedVehicleSizeDtoRequest.Filter.PageNumber - 1) * paginatedVehicleSizeDtoRequest.Filter.PageSize).Take(paginatedVehicleSizeDtoRequest.Filter.PageSize).ToList();

                paginatedVehicleSizeDtoResponse.VehicleSizes = vehicleSizeDtoList;
                return paginatedVehicleSizeDtoResponse;
            },
            new CacheOptions
            {
                ListType = CacheListType.VehicleSize,
                AbsoluteExpireTime = TimeSpan.FromMinutes(5)
            }
        );
    }

    public async Task<GetVehicleSizeDtoResponse> GetByIdAsync(GetVehicleSizeDtoRequest getVehicleSizeDtoRequest)
    {
        var getVehicleSizeDtoResponse = new GetVehicleSizeDtoResponse();

        var validator = new GetVehicleSizeValidator();
        var result = validator.Validate(getVehicleSizeDtoRequest);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        return await cacheHandler.GetOrCreateRecordAsync(
            getVehicleSizeDtoRequest,
            async () =>
            {
                getVehicleSizeDtoResponse.VehicleSize = await vehicleSizeRepository.GetByIdAsync(getVehicleSizeDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.VehicleSizeNotFound);
                return getVehicleSizeDtoResponse;
            },
            new CacheOptions
            {
                Id = getVehicleSizeDtoRequest.Id,
                AbsoluteExpireTime = TimeSpan.FromDays(1)
            }
        );
    }
}