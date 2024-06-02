using AutoMapper;

using System.Net;

using Valeting.Common.Messages;
using Valeting.Services.Validators;
using Valeting.Services.Interfaces;
using Valeting.Business.VehicleSize;
using Valeting.Repositories.Interfaces;
using Valeting.Services.Objects.VehicleSize;

namespace Valeting.Services;

public class VehicleSizeService(IVehicleSizeRepository vehicleSizeRepository, IMapper mapper) : IVehicleSizeService
{
    public async Task<PaginatedVehicleSizeSVResponse> GetAsync(PaginatedVehicleSizeSVRequest paginatedVehicleSizeSVRequest)
    {
        var paginatedVehicleSizeSVResponse = new PaginatedVehicleSizeSVResponse();

        var validator = new PaginatedVehicleSizeValidator();
        var result = validator.Validate(paginatedVehicleSizeSVRequest);
        if(!result.IsValid)
        {
            paginatedVehicleSizeSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return paginatedVehicleSizeSVResponse;
        }

        var vehicleSizeFilterDTO = mapper.Map<VehicleSizeFilterDTO>(paginatedVehicleSizeSVRequest.Filter);

        var vehicleSizeListDTO = await vehicleSizeRepository.GetAsync(vehicleSizeFilterDTO);
        if(vehicleSizeListDTO == null)
        {
            paginatedVehicleSizeSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.VehicleSizeNotFound
            };
            return paginatedVehicleSizeSVResponse;
        }

        return mapper.Map<PaginatedVehicleSizeSVResponse>(vehicleSizeListDTO);
    }

    public async Task<GetVehicleSizeSVResponse> GetByIdAsync(GetVehicleSizeSVRequest getVehicleSizeSVRequest)
    {
        var getVehicleSizeSVResponse = new GetVehicleSizeSVResponse();

        var validator = new GetVehicleSizeValidator();
        var result = validator.Validate(getVehicleSizeSVRequest);
        if (!result.IsValid)
        {
            getVehicleSizeSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return getVehicleSizeSVResponse;
        }

        var vehicleSizeDTO = await vehicleSizeRepository.GetByIDAsync(getVehicleSizeSVRequest.Id);
        if (vehicleSizeDTO == null)
        {
            getVehicleSizeSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.VehicleSizeNotFound
            };
            return getVehicleSizeSVResponse;
        }

        return mapper.Map<GetVehicleSizeSVResponse>(vehicleSizeDTO);
    }
}