﻿using Valeting.Common.Cache;
using Valeting.Common.Cache.Interfaces;
using Valeting.Common.Messages;
using Valeting.Common.Models.VehicleSize;
using Valeting.Core.Interfaces;
using Valeting.Core.Validators;
using Valeting.Core.Validators.Utils;
using Valeting.Repository.Interfaces;

namespace Valeting.Core.Services;

public class VehicleSizeService(IVehicleSizeRepository vehicleSizeRepository, ICacheHandler cacheHandler) : IVehicleSizeService
{
    public async Task<PaginatedVehicleSizeDtoResponse> GetFilteredAsync(PaginatedVehicleSizeDtoRequest paginatedVehicleSizeDtoRequest)
    {
        var paginatedVehicleSizeDtoResponse = new PaginatedVehicleSizeDtoResponse();

        paginatedVehicleSizeDtoRequest.ValidateRequest(new PaginatedVehicleSizeValidator());

        return await cacheHandler.GetOrCreateRecordAsync(
            paginatedVehicleSizeDtoRequest,
            async () =>
            {
                var vehicleSizeDtoList = await vehicleSizeRepository.GetFilteredAsync(paginatedVehicleSizeDtoRequest.Filter);
                if (vehicleSizeDtoList.Count == 0)
                    throw new KeyNotFoundException(Messages.NotFound);

                paginatedVehicleSizeDtoResponse.TotalItems = vehicleSizeDtoList.Count();
                paginatedVehicleSizeDtoResponse.TotalPages = (int)Math.Ceiling((double)paginatedVehicleSizeDtoResponse.TotalItems / paginatedVehicleSizeDtoRequest.Filter.PageSize);

                vehicleSizeDtoList = vehicleSizeDtoList
                    .OrderBy(x => x.Id)
                    .Skip((paginatedVehicleSizeDtoRequest.Filter.PageNumber - 1) * paginatedVehicleSizeDtoRequest.Filter.PageSize)
                    .Take(paginatedVehicleSizeDtoRequest.Filter.PageSize)
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

    public async Task<GetVehicleSizeDtoResponse> GetByIdAsync(GetVehicleSizeDtoRequest getVehicleSizeDtoRequest)
    {
        var getVehicleSizeDtoResponse = new GetVehicleSizeDtoResponse();

        getVehicleSizeDtoRequest.ValidateRequest(new GetVehicleSizeValidator());

        return await cacheHandler.GetOrCreateRecordAsync(
            getVehicleSizeDtoRequest,
            async () =>
            {
                getVehicleSizeDtoResponse.VehicleSize = await vehicleSizeRepository.GetByIdAsync(getVehicleSizeDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.NotFound);
                return getVehicleSizeDtoResponse;
            },
            new()
            {
                Id = getVehicleSizeDtoRequest.Id,
                AbsoluteExpireTime = TimeSpan.FromDays(1)
            }
        );
    }
}