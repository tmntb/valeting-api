using System.Net;

using Valeting.Common.Messages;
using Valeting.Services.Validators;
using Valeting.Services.Interfaces;
using Valeting.Business.VehicleSize;
using Valeting.Repositories.Interfaces;
using Valeting.Services.Objects.VehicleSize;

namespace Valeting.Services;

public class VehicleSizeService(IVehicleSizeRepository vehicleSizeRepository) : IVehicleSizeService
{
    public async Task<GetVehicleSizeSVResponse> GetAsync(GetVehicleSizeSVRequest getVehicleSizeSVRequest)
    {
        var getVehicleSizeSVResponse = new GetVehicleSizeSVResponse();

        var validator = new GetVehicleSizeValidator();
        var result = validator.Validate(getVehicleSizeSVRequest);
        if(!result.IsValid)
        {
            getVehicleSizeSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return getVehicleSizeSVResponse;
        }

        var vehicleSizeDTO = await vehicleSizeRepository.FindByIDAsync(getVehicleSizeSVRequest.Id);
        if (vehicleSizeDTO == null)
        {
            getVehicleSizeSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.VehicleSizeNotFound
            };
            return getVehicleSizeSVResponse;
        }

        getVehicleSizeSVResponse.Id = vehicleSizeDTO.Id;
        getVehicleSizeSVResponse.Description = vehicleSizeDTO.Description;
        getVehicleSizeSVResponse.Active = vehicleSizeDTO.Active;
        return getVehicleSizeSVResponse;
    }

    public async Task<PaginatedVehicleSizeSVResponse> ListAllAsync(PaginatedVehicleSizeSVRequest paginatedVehicleSizeSVRequest)
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

        var vehicleSizeFilterDTO = new VehicleSizeFilterDTO()
        {
            PageNumber = paginatedVehicleSizeSVRequest.Filter.PageNumber,
            PageSize = paginatedVehicleSizeSVRequest.Filter.PageSize,
            Active = paginatedVehicleSizeSVRequest.Filter.Active
        };
        
        var vehicleSizeListDTO = await vehicleSizeRepository.ListAsync(vehicleSizeFilterDTO);
        if(vehicleSizeListDTO == null)
        {
            paginatedVehicleSizeSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.VehicleSizeNotFound
            };
            return paginatedVehicleSizeSVResponse;
        }

        paginatedVehicleSizeSVResponse.TotalItems = vehicleSizeListDTO.TotalItems;
        paginatedVehicleSizeSVResponse.TotalPages = vehicleSizeListDTO.TotalPages;

        paginatedVehicleSizeSVResponse.VehicleSizes = vehicleSizeListDTO.VehicleSizes.Select(x => 
            new VehicleSizeSV()
            {
                Id = x.Id,
                Description = x.Description,
                Active = x.Active
            }
        ).ToList();
        return paginatedVehicleSizeSVResponse;
    }
}