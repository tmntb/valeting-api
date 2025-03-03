using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.ComponentModel.DataAnnotations;
using Valeting.Models.Core;
using Valeting.Core.Interfaces;
using Valeting.Cache.Interfaces;
using Valeting.Common.Models.Link;
using Valeting.Models.VehicleSize;
using Valeting.Common.Models.VehicleSize;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers;

public class VehicleSizeController(IVehicleSizeService vehicleSizeService, IUrlService urlService, ICacheHandler cacheHandler, IMapper mapper) : VehicleSizeBaseController
{
    public override async Task<IActionResult> GetAsync([FromQuery] VehicleSizeApiParameters vehicleSizeApiParameters)
    {
        try
        {
            var paginatedVehicleSizeDtoRequest = mapper.Map<PaginatedVehicleSizeDtoRequest>(vehicleSizeApiParameters);

            var recordKey = string.Format("ListVehicleSize_{0}_{1}_{2}", vehicleSizeApiParameters.PageNumber, vehicleSizeApiParameters.PageSize, vehicleSizeApiParameters.Active);
            var paginatedVehicleSizeDtoResponse = cacheHandler.GetRecord<PaginatedVehicleSizeDtoResponse>(recordKey);
            if (paginatedVehicleSizeDtoResponse == null)
            {
                paginatedVehicleSizeDtoResponse = await vehicleSizeService.GetAsync(paginatedVehicleSizeDtoRequest);
                if (paginatedVehicleSizeDtoResponse.HasError)
                {
                    var vehicleSizeApiError = new VehicleSizeApiError
                    {
                        Detail = paginatedVehicleSizeDtoResponse.Error.Message
                    };
                    return StatusCode(paginatedVehicleSizeDtoResponse.Error.ErrorCode, vehicleSizeApiError);
                }

                cacheHandler.SetRecord(recordKey, paginatedVehicleSizeDtoResponse, TimeSpan.FromMinutes(5));
            }

            var vehicleSizeApiPaginatedResponse = new VehicleSizeApiPaginatedResponse
            {
                VehicleSizes = [],
                CurrentPage = vehicleSizeApiParameters.PageNumber,
                TotalItems = paginatedVehicleSizeDtoResponse.TotalItems,
                TotalPages = paginatedVehicleSizeDtoResponse.TotalPages,
                Links = new()
                {
                    Prev = new() { Href = string.Empty },
                    Next = new() { Href = string.Empty },
                    Self = new() { Href = string.Empty }
                },
            };

            var paginatedLinks = urlService.GeneratePaginatedLinks
            (
                new GeneratePaginatedLinksDtoRequest
                {
                    BaseUrl = Request.Host.Value,
                    Path = Request.Path.HasValue ? Request.Path.Value : string.Empty,
                    QueryString = Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty,
                    PageNumber = vehicleSizeApiParameters.PageNumber,
                    TotalPages = paginatedVehicleSizeDtoResponse.TotalPages,
                    Filter = paginatedVehicleSizeDtoRequest.Filter
                }
            );

            var links = mapper.Map<PaginationLinksApi>(paginatedLinks);
            vehicleSizeApiPaginatedResponse.Links = links;

            var vehicleSizeApis = mapper.Map<List<VehicleSizeApi>>(paginatedVehicleSizeDtoResponse.VehicleSizes);
            vehicleSizeApis.ForEach(v => 
                v.Link = new() 
                { 
                    Self = new() 
                    { 
                        Href = urlService.GenerateSelf(new GenerateSelfUrlDtoRequest { BaseUrl = Request.Host.Value, Path = Request.Path.Value, Id = v.Id }).Self 
                    } 
                }
            );
            vehicleSizeApiPaginatedResponse.VehicleSizes = vehicleSizeApis;

            return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiPaginatedResponse);
        }
        catch (Exception ex)
        {
            var vehicleSizeApiError = new VehicleSizeApiError
            {
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, vehicleSizeApiError);
        }
    }

    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        try
        {
            var getVehicleSizeDtoRequest = new GetVehicleSizeDtoRequest
            {
                Id = Guid.Parse(id)
            };

            var recordKey = string.Format("VehicleSize_{0}", id);
            var getVehicleSizeDtoResponse = cacheHandler.GetRecord<GetVehicleSizeDtoResponse>(recordKey);
            if (getVehicleSizeDtoResponse == null)
            {
                getVehicleSizeDtoResponse = await vehicleSizeService.GetByIdAsync(getVehicleSizeDtoRequest);
                if (getVehicleSizeDtoResponse.HasError)
                {
                    var vehicleSizeApiError = new VehicleSizeApiError
                    {
                        Detail = getVehicleSizeDtoResponse.Error.Message
                    };
                    return StatusCode(getVehicleSizeDtoResponse.Error.ErrorCode, vehicleSizeApiError);
                }

                cacheHandler.SetRecord(recordKey, getVehicleSizeDtoResponse, TimeSpan.FromDays(1));
            }

            var vehicleSizeApi = mapper.Map<VehicleSizeApi>(getVehicleSizeDtoResponse.VehicleSize);
            vehicleSizeApi.Link = new() 
            { 
                Self = new() 
                { 
                    Href = urlService.GenerateSelf(new GenerateSelfUrlDtoRequest { BaseUrl = Request.Host.Value, Path = Request.Path.HasValue ? Request.Path.Value : string.Empty }).Self 
                } 
            };

            var vehicleSizeApiResponse = new VehicleSizeApiResponse
            {
                VehicleSize = vehicleSizeApi
            };
            return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiResponse);
        }
        catch (Exception ex)
        {
            var vehicleSizeApiError = new VehicleSizeApiError
            {
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, vehicleSizeApiError);
        }
    }
}