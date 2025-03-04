using AutoMapper;
using FluentValidation;
using Valeting.Common.Cache;
using Valeting.Core.Interfaces;
using Valeting.Common.Messages;
using Valeting.Services.Validators;
using Valeting.Repository.Interfaces;
using Valeting.Common.Cache.Interfaces;
using Valeting.Common.Models.VehicleSize;

namespace Valeting.Core.Services;

public class VehicleSizeService(IVehicleSizeRepository vehicleSizeRepository, ICacheHandler cacheHandler, IMapper mapper) : IVehicleSizeService
{
    public async Task<PaginatedVehicleSizeDtoResponse> GetAsync(PaginatedVehicleSizeDtoRequest paginatedVehicleSizeDtoRequest)
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
                var vehicleSizeFilterDto = mapper.Map<VehicleSizeFilterDto>(paginatedVehicleSizeDtoRequest.Filter);
                var vehicleSizeListDto = await vehicleSizeRepository.GetAsync(vehicleSizeFilterDto) ?? throw new KeyNotFoundException(Messages.VehicleSizeNotFound);
                return mapper.Map<PaginatedVehicleSizeDtoResponse>(vehicleSizeListDto);
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
                var vehicleSizeDto = await vehicleSizeRepository.GetByIdAsync(getVehicleSizeDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.VehicleSizeNotFound);
                getVehicleSizeDtoResponse.VehicleSize = mapper.Map<VehicleSizeDto>(vehicleSizeDto);
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