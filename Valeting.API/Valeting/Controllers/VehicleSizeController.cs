using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.Helpers.Interfaces;
using Valeting.Services.Interfaces;
using Valeting.ApiObjects.VehicleSize;
using Valeting.Controllers.BaseController;
using Valeting.Services.Objects.VehicleSize;

namespace Valeting.Controllers;

public class VehicleSizeController(IRedisCache redisCache, IVehicleSizeService vehicleSizeService, IUrlService urlService) : VehicleSizeBaseController
{
    public override async Task<IActionResult> FindByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        try
        {
            var getVehicleSizeSVRequest = new GetVehicleSizeSVRequest()
            {
                Id = Guid.Parse(id)
            };

            var recordKey = string.Format("VehicleSize_{0}", id);
            var getVehicleSizeSVResponse = await redisCache.GetRecordAsync<GetVehicleSizeSVResponse>(recordKey);
            if(getVehicleSizeSVResponse == null)
            {
                getVehicleSizeSVResponse = await vehicleSizeService.GetAsync(getVehicleSizeSVRequest);
                if(getVehicleSizeSVResponse.HasError)
                {
                    var vehicleSizeApiError = new VehicleSizeApiError() 
                    { 
                        Detail = getVehicleSizeSVResponse.Error.Message
                    };
                    return StatusCode(getVehicleSizeSVResponse.Error.ErrorCode, vehicleSizeApiError);
                }

                await redisCache.SetRecordAsync(recordKey, getVehicleSizeSVResponse, TimeSpan.FromDays(1));
            }

            var vehicleSizeApiResponse = new VehicleSizeApiResponse()
            {
                VehicleSize = new()
                {
                    Id = getVehicleSizeSVResponse.Id,
                    Description = getVehicleSizeSVResponse.Description,
                    Active = getVehicleSizeSVResponse.Active,
                    Link = new()
                    {
                        Self = new()
                        {
                            Href = urlService.GenerateSelf(Request.Host.Value, Request.Path.HasValue ? Request.Path.Value : string.Empty)
                        }
                    }
                }
            };
            return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiResponse);
        }
        catch (Exception ex)
        {
            var vehicleSizeApiError = new VehicleSizeApiError() 
            { 
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, vehicleSizeApiError);
        }
    }

    public override async Task<IActionResult> ListAllAsync([FromQuery] VehicleSizeApiParameters vehicleSizeApiParameters)
    {
        try
        {
            var paginatedVehicleSizeSVRequest = new PaginatedVehicleSizeSVRequest()
            {
                Filter = new()
                {
                    PageNumber = vehicleSizeApiParameters.PageNumber,
                    PageSize = vehicleSizeApiParameters.PageSize,
                    Active = vehicleSizeApiParameters.Active
                }
            };

            var recordKey = string.Format("ListVehicleSize_{0}_{1}_{2}", vehicleSizeApiParameters.PageNumber, vehicleSizeApiParameters.PageSize, vehicleSizeApiParameters.Active);
            var paginatedVehicleSizeSVResponse = await redisCache.GetRecordAsync<PaginatedVehicleSizeSVResponse>(recordKey);
            if (paginatedVehicleSizeSVResponse == null)
            {
                paginatedVehicleSizeSVResponse = await vehicleSizeService.ListAllAsync(paginatedVehicleSizeSVRequest);
                if(paginatedVehicleSizeSVResponse.HasError)
                {
                    var vehicleSizeApiError = new VehicleSizeApiError()
                    {
                        Detail = paginatedVehicleSizeSVResponse.Error.Message
                    };
                    return StatusCode(paginatedVehicleSizeSVResponse.Error.ErrorCode, vehicleSizeApiError);
                }

                await redisCache.SetRecordAsync(recordKey, paginatedVehicleSizeSVResponse, TimeSpan.FromMinutes(5));
            }

            var vehicleSizeApiPaginatedResponse = new VehicleSizeApiPaginatedResponse
            {
                VehicleSizes = [],
                CurrentPage = vehicleSizeApiParameters.PageNumber,
                TotalItems = paginatedVehicleSizeSVResponse.TotalItems,
                TotalPages = paginatedVehicleSizeSVResponse.TotalPages,
                Links = new()
                {
                    Prev = new() { Href = string.Empty },
                    Next = new() { Href = string.Empty },
                    Self = new() { Href = string.Empty }
                },
            };

            var linkDTO = urlService.GeneratePaginatedLinks
            (
                Request.Host.Value,
                Request.Path.HasValue ? Request.Path.Value : string.Empty,
                Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty,
                vehicleSizeApiParameters.PageNumber, 
                paginatedVehicleSizeSVResponse.TotalPages, 
                paginatedVehicleSizeSVRequest.Filter
            );

            vehicleSizeApiPaginatedResponse.Links.Prev.Href = linkDTO.Prev;
            vehicleSizeApiPaginatedResponse.Links.Next.Href = linkDTO.Next;
            vehicleSizeApiPaginatedResponse.Links.Self.Href = linkDTO.Self;

            vehicleSizeApiPaginatedResponse.VehicleSizes.AddRange(
                paginatedVehicleSizeSVResponse.VehicleSizes.Select(item => 
                    new VehicleSizeApi()
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Active = item.Active,
                        Link = new()
                        {
                            Self = new()
                            {
                                Href = urlService.GenerateSelf(Request.Host.Value, Request.Path.Value, item.Id)
                            }
                        }
                    }
                ).ToList()
            );
            return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiPaginatedResponse);
        }
        catch (Exception ex)
        {
            var vehicleSizeApiError = new VehicleSizeApiError() 
            { 
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, vehicleSizeApiError);
        }
    }
}