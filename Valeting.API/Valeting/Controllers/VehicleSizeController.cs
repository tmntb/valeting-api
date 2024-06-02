using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.ComponentModel.DataAnnotations;

using Valeting.Helpers.Interfaces;
using Valeting.Services.Interfaces;
using Valeting.Services.Objects.Link;
using Valeting.ApiObjects.VehicleSize;
using Valeting.Controllers.BaseController;
using Valeting.Services.Objects.VehicleSize;

namespace Valeting.Controllers;

public class VehicleSizeController(IRedisCache redisCache, IVehicleSizeService vehicleSizeService, IUrlService urlService, IMapper mapper) : VehicleSizeBaseController
{
    public override async Task<IActionResult> GetAsync([FromQuery] VehicleSizeApiParameters vehicleSizeApiParameters)
    {
        try
        {
            var paginatedVehicleSizeSVRequest = mapper.Map<PaginatedVehicleSizeSVRequest>(vehicleSizeApiParameters);

            var recordKey = string.Format("ListVehicleSize_{0}_{1}_{2}", vehicleSizeApiParameters.PageNumber, vehicleSizeApiParameters.PageSize, vehicleSizeApiParameters.Active);
            var paginatedVehicleSizeSVResponse = await redisCache.GetRecordAsync<PaginatedVehicleSizeSVResponse>(recordKey);
            if (paginatedVehicleSizeSVResponse == null)
            {
                paginatedVehicleSizeSVResponse = await vehicleSizeService.GetAsync(paginatedVehicleSizeSVRequest);
                if (paginatedVehicleSizeSVResponse.HasError)
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

            var paginatedLinks = urlService.GeneratePaginatedLinks
            (
                new GeneratePaginatedLinksSVRequest()
                {
                    BaseUrl = Request.Host.Value,
                    Path = Request.Path.HasValue ? Request.Path.Value : string.Empty,
                    QueryString = Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty,
                    PageNumber = vehicleSizeApiParameters.PageNumber,
                    TotalPages = paginatedVehicleSizeSVResponse.TotalPages,
                    Filter = paginatedVehicleSizeSVRequest.Filter
                }
            );

            vehicleSizeApiPaginatedResponse.Links.Prev.Href = paginatedLinks.Prev;
            vehicleSizeApiPaginatedResponse.Links.Next.Href = paginatedLinks.Next;
            vehicleSizeApiPaginatedResponse.Links.Self.Href = paginatedLinks.Self;

            var vehicleSizeApis = mapper.Map<List<VehicleSizeApi>>(paginatedVehicleSizeSVResponse.VehicleSizes);
            vehicleSizeApis.ForEach(v => 
                v.Link = new() 
                { 
                    Self = new() 
                    { 
                        Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = Request.Path.Value, Id = v.Id }).Self 
                    } 
                }
            );
            vehicleSizeApiPaginatedResponse.VehicleSizes = vehicleSizeApis;

            return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiPaginatedResponse);
        }
        catch (Exception ex)
        {
            var vehicleSizeApiError = new VehicleSizeApiError()
            {
                Detail = ex.StackTrace
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, vehicleSizeApiError);
        }
    }

    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        try
        {
            var getVehicleSizeSVRequest = new GetVehicleSizeSVRequest()
            {
                Id = Guid.Parse(id)
            };

            var recordKey = string.Format("VehicleSize_{0}", id);
            var getVehicleSizeSVResponse = await redisCache.GetRecordAsync<GetVehicleSizeSVResponse>(recordKey);
            if (getVehicleSizeSVResponse == null)
            {
                getVehicleSizeSVResponse = await vehicleSizeService.GetByIdAsync(getVehicleSizeSVRequest);
                if (getVehicleSizeSVResponse.HasError)
                {
                    var vehicleSizeApiError = new VehicleSizeApiError()
                    {
                        Detail = getVehicleSizeSVResponse.Error.Message
                    };
                    return StatusCode(getVehicleSizeSVResponse.Error.ErrorCode, vehicleSizeApiError);
                }

                await redisCache.SetRecordAsync(recordKey, getVehicleSizeSVResponse, TimeSpan.FromDays(1));
            }

            var vehicleSizeApi = mapper.Map<VehicleSizeApi>(getVehicleSizeSVResponse.VehicleSize);
            vehicleSizeApi.Link = new() 
            { 
                Self = new() 
                { 
                    Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = Request.Path.HasValue ? Request.Path.Value : string.Empty }).Self 
                } 
            };

            var vehicleSizeApiResponse = new VehicleSizeApiResponse()
            {
                VehicleSize = vehicleSizeApi
            };
            return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiResponse);
        }
        catch (Exception ex)
        {
            var vehicleSizeApiError = new VehicleSizeApiError()
            {
                Detail = ex.StackTrace
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, vehicleSizeApiError);
        }
    }
}