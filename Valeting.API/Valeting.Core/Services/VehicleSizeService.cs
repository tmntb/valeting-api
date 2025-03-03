using AutoMapper;
using System.Net;
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
            paginatedVehicleSizeDtoResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return paginatedVehicleSizeDtoResponse;
        }

        var recordKey = string.Format("ListVehicleSize_{0}_{1}_{2}", paginatedVehicleSizeDtoRequest.Filter.PageNumber, paginatedVehicleSizeDtoRequest.Filter.PageSize, paginatedVehicleSizeDtoRequest.Filter.Active);
        paginatedVehicleSizeDtoResponse = cacheHandler.GetRecord<PaginatedVehicleSizeDtoResponse>(recordKey);
        if (paginatedVehicleSizeDtoResponse == null)
        {
            var vehicleSizeFilterDto = mapper.Map<VehicleSizeFilterDto>(paginatedVehicleSizeDtoRequest.Filter);

            var vehicleSizeListDto = await vehicleSizeRepository.GetAsync(vehicleSizeFilterDto);
            if (vehicleSizeListDto == null)
            {
                paginatedVehicleSizeDtoResponse.Error = new()
                {
                    ErrorCode = (int)HttpStatusCode.NotFound,
                    Message = Messages.VehicleSizeNotFound
                };
                return paginatedVehicleSizeDtoResponse;
            }

            paginatedVehicleSizeDtoResponse = mapper.Map<PaginatedVehicleSizeDtoResponse>(vehicleSizeListDto);

            cacheHandler.SetRecord(recordKey, paginatedVehicleSizeDtoResponse, TimeSpan.FromMinutes(5));
        }

        return paginatedVehicleSizeDtoResponse;
    }

    public async Task<GetVehicleSizeDtoResponse> GetByIdAsync(GetVehicleSizeDtoRequest getVehicleSizeDtoRequest)
    {
        var getVehicleSizeDtoResponse = new GetVehicleSizeDtoResponse();

        var validator = new GetVehicleSizeValidator();
        var result = validator.Validate(getVehicleSizeDtoRequest);
        if (!result.IsValid)
        {
            getVehicleSizeDtoResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return getVehicleSizeDtoResponse;
        }

        var recordKey = string.Format("VehicleSize_{0}", getVehicleSizeDtoRequest.Id);
        getVehicleSizeDtoResponse = cacheHandler.GetRecord<GetVehicleSizeDtoResponse>(recordKey);
        if (getVehicleSizeDtoResponse == null)
        {
            var vehicleSizeDto = await vehicleSizeRepository.GetByIdAsync(getVehicleSizeDtoRequest.Id);
            if (vehicleSizeDto == null)
            {
                getVehicleSizeDtoResponse.Error = new()
                {
                    ErrorCode = (int)HttpStatusCode.NotFound,
                    Message = Messages.VehicleSizeNotFound
                };
                return getVehicleSizeDtoResponse;
            }

            getVehicleSizeDtoResponse.VehicleSize = mapper.Map<VehicleSizeDto>(vehicleSizeDto);

            cacheHandler.SetRecord(recordKey, getVehicleSizeDtoResponse, TimeSpan.FromDays(1));
        }

        return getVehicleSizeDtoResponse;
    }
}