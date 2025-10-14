using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Api.Controllers.BaseController;
using Api.Models.Core;
using Api.Models.VehicleSize;
using Common.Messages;
using Common.Models.VehicleSize;
using Service.Interfaces;

namespace Api.Controllers;

public class VehicleSizeController(IVehicleSizeService vehicleSizeService, IUrlService urlService) : VehicleSizeBaseController
{
    public override async Task<IActionResult> GetFilteredAsync([FromQuery] VehicleSizeApiParameters vehicleSizeApiParameters)
    {
        ArgumentNullException.ThrowIfNull(vehicleSizeApiParameters, Messages.InvalidRequestQueryParameters);

        var paginatedVehicleSizeDtoRequest = new PaginatedVehicleSizeDtoRequest 
        {
            Filter = new()
            {
                PageNumber = vehicleSizeApiParameters.PageNumber,
                PageSize = vehicleSizeApiParameters.PageSize,
                Active = vehicleSizeApiParameters.Active                
            }
        };

        var paginatedVehicleSizeDtoResponse = await vehicleSizeService.GetFilteredAsync(paginatedVehicleSizeDtoRequest);
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
            new()
            {
                Request = Request,
                TotalPages = paginatedVehicleSizeDtoResponse.TotalPages,
                Filter = paginatedVehicleSizeDtoRequest.Filter
            }
        );

        var links = new PaginationLinksApi { };
        vehicleSizeApiPaginatedResponse.Links = links;

        var vehicleSizeApis = paginatedVehicleSizeDtoResponse.VehicleSizes.Select(x =>
            new VehicleSizeApi()
            {
                Id = x.Id,
                Description = x.Description,
                Active = x.Active
            }
        ).ToList();

        vehicleSizeApis.ForEach(v =>
            v.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new() { Request = Request, Id = v.Id }).Self
                }
            }
        );

        vehicleSizeApiPaginatedResponse.VehicleSizes = vehicleSizeApis;
        return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiPaginatedResponse);
    }

    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        ArgumentNullException.ThrowIfNull(id, Messages.InvalidRequestId);

        var getVehicleSizeDtoRequest = new GetVehicleSizeDtoRequest
        {
            Id = Guid.Parse(id)
        };

        var getVehicleSizeDtoResponse = await vehicleSizeService.GetByIdAsync(getVehicleSizeDtoRequest);

        var vehicleSizeApi = new VehicleSizeApi
        {
            Id = getVehicleSizeDtoResponse.VehicleSize.Id,
            Description = getVehicleSizeDtoResponse.VehicleSize.Description,
            Active = getVehicleSizeDtoResponse.VehicleSize.Active,
            Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new() { Request = Request }).Self
                }
            }
        };

        var vehicleSizeApiResponse = new VehicleSizeApiResponse
        {
            VehicleSize = vehicleSizeApi
        };
        return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiResponse);
    }
}