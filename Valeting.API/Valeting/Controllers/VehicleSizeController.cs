using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.ComponentModel.DataAnnotations;
using Valeting.Models.Core;
using Valeting.Core.Interfaces;
using Valeting.Common.Models.Link;
using Valeting.Models.VehicleSize;
using Valeting.Common.Models.VehicleSize;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers;

public class VehicleSizeController(IVehicleSizeService vehicleSizeService, IUrlService urlService, IMapper mapper) : VehicleSizeBaseController
{
    public override async Task<IActionResult> GetAsync([FromQuery] VehicleSizeApiParameters vehicleSizeApiParameters)
    {
        ArgumentNullException.ThrowIfNull(vehicleSizeApiParameters, "Invalid api parameters");

        var paginatedVehicleSizeDtoRequest = mapper.Map<PaginatedVehicleSizeDtoRequest>(vehicleSizeApiParameters);

        var paginatedVehicleSizeDtoResponse = await vehicleSizeService.GetAsync(paginatedVehicleSizeDtoRequest);
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

    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        ArgumentNullException.ThrowIfNull(id, "Invalid request id");

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
                Href = urlService.GenerateSelf(new GenerateSelfUrlDtoRequest { BaseUrl = Request.Host.Value, Path = Request.Path.HasValue ? Request.Path.Value : string.Empty }).Self
            }
        };

        var vehicleSizeApiResponse = new VehicleSizeApiResponse
        {
            VehicleSize = vehicleSizeApi
        };
        return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiResponse);
    }
}