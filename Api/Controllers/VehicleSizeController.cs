using Api.Controllers.BaseController;
using Api.Models.Core;
using Api.Models.VehicleSize;
using Api.Models.VehicleSize.Payload;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.VehicleSize.Payload;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Api.Controllers;

public class VehicleSizeController(IVehicleSizeService vehicleSizeService, ILinkService urlService) : VehicleSizeBaseController
{
    /// <inheritdoc />
    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        ArgumentNullException.ThrowIfNull(id, Messages.InvalidRequestId);

        var vehicleSizeDto = await vehicleSizeService.GetByIdAsync(Guid.Parse(id));

        var vehicleSizeApi = new VehicleSizeApi
        {
            Id = vehicleSizeDto.Id,
            Description = vehicleSizeDto.Description,
            Active = vehicleSizeDto.Active,
            Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new() { Request = Request })
                }
            }
        };

        var vehicleSizeApiResponse = new VehicleSizeApiResponse
        {
            VehicleSize = vehicleSizeApi
        };
        return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiResponse);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetFilteredAsync([FromQuery] VehicleSizeApiParameters vehicleSizeApiParameters)
    {
        ArgumentNullException.ThrowIfNull(vehicleSizeApiParameters, Messages.InvalidRequestQueryParameters);

        var vehicleSizeFilterDto = new VehicleSizeFilterDto
        {
            PageNumber = vehicleSizeApiParameters.PageNumber,
            PageSize = vehicleSizeApiParameters.PageSize,
            Active = vehicleSizeApiParameters.Active
        };

        var paginatedVehicleSizeDtoResponse = await vehicleSizeService.GetFilteredAsync(vehicleSizeFilterDto);
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
                Filter = vehicleSizeFilterDto
            }
        );

        var links = new PaginationLinksApi
        {
            Self = new() { Href = paginatedLinks.Self },
            Next = new() { Href = paginatedLinks.Next },
            Prev = new() { Href = paginatedLinks.Prev }
        };
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
                    Href = urlService.GenerateSelf(new() { Request = Request, Id = v.Id })
                }
            }
        );

        vehicleSizeApiPaginatedResponse.VehicleSizes = vehicleSizeApis;
        return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiPaginatedResponse);
    }
}