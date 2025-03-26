using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Valeting.API.Controllers.BaseController;
using Valeting.API.Models.Core;
using Valeting.API.Models.VehicleSize;
using Valeting.Common.Messages;
using Valeting.Common.Models.VehicleSize;
using Valeting.Core.Interfaces;

namespace Valeting.API.Controllers;

public class VehicleSizeController(IVehicleSizeService vehicleSizeService, IUrlService urlService, IMapper mapper) : VehicleSizeBaseController
{
    public override async Task<IActionResult> GetFilteredAsync([FromQuery] VehicleSizeApiParameters vehicleSizeApiParameters)
    {
        ArgumentNullException.ThrowIfNull(vehicleSizeApiParameters, Messages.InvalidRequestQueryParameters);

        var paginatedVehicleSizeDtoRequest = mapper.Map<PaginatedVehicleSizeDtoRequest>(vehicleSizeApiParameters);

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

        var links = mapper.Map<PaginationLinksApi>(paginatedLinks);
        vehicleSizeApiPaginatedResponse.Links = links;

        var vehicleSizeApis = mapper.Map<List<VehicleSizeApi>>(paginatedVehicleSizeDtoResponse.VehicleSizes);
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

        var vehicleSizeApi = mapper.Map<VehicleSizeApi>(getVehicleSizeDtoResponse.VehicleSize);
        vehicleSizeApi.Link = new()
        {
            Self = new()
            {
                Href = urlService.GenerateSelf(new() { Request = Request }).Self
            }
        };

        var vehicleSizeApiResponse = new VehicleSizeApiResponse
        {
            VehicleSize = vehicleSizeApi
        };
        return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiResponse);
    }
}