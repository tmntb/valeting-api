using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Api.Controllers.BaseController;
using Api.Models.Core;
using Api.Models.Flexibility;
using Common.Messages;
using Common.Models.Flexibility;
using Service.Interfaces;

namespace Api.Controllers;

public class FlexibilityController(IFlexibilityService flexibilityService, IUrlService urlService) : FlexibilityBaseController
{
    public override async Task<IActionResult> GetFilteredAsync([FromQuery] FlexibilityApiParameters flexibilityApiParameters)
    {
        ArgumentNullException.ThrowIfNull(flexibilityApiParameters, Messages.InvalidRequestQueryParameters);

        var paginatedFlexibilityDtoRequest = new PaginatedFlexibilityDtoRequest 
        { 
            Filter = new()
            {
                PageNumber = flexibilityApiParameters.PageNumber,
                PageSize = flexibilityApiParameters.PageSize,
                Active = flexibilityApiParameters.Active
            }
        };
        var paginatedFlexibilityDtoResponse = await flexibilityService.GetFilteredAsync(paginatedFlexibilityDtoRequest);

        var flexibilityApiPaginatedResponse = new FlexibilityApiPaginatedResponse
        {
            Flexibilities = [],
            CurrentPage = flexibilityApiParameters.PageNumber,
            TotalItems = paginatedFlexibilityDtoResponse.TotalItems,
            TotalPages = paginatedFlexibilityDtoResponse.TotalPages,
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
                TotalPages = paginatedFlexibilityDtoResponse.TotalPages,
                Filter = paginatedFlexibilityDtoRequest.Filter
            }
        );

        var links = new PaginationLinksApi { };
        flexibilityApiPaginatedResponse.Links = links;

        var flexibilityApis = paginatedFlexibilityDtoResponse.Flexibilities.Select(x => 
            new FlexibilityApi()
            {
                Id = x.Id,
                Description = x.Description,
                Active = x.Active
            }
        ).ToList();

        flexibilityApis.ForEach(f =>
            f.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new() { Request = Request, Id = f.Id }).Self
                }
            }
        );

        flexibilityApiPaginatedResponse.Flexibilities = flexibilityApis;
        return StatusCode((int)HttpStatusCode.OK, flexibilityApiPaginatedResponse);
    }

    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        ArgumentNullException.ThrowIfNull(id, Messages.InvalidRequestId);

        var getFlexibilityDtoRequest = new GetFlexibilityDtoRequest
        {
            Id = Guid.Parse(id)
        };

        var getFlexibilityDtoResponse = await flexibilityService.GetByIdAsync(getFlexibilityDtoRequest);

        var flexibilityApi = new FlexibilityApi
        {
            Id = getFlexibilityDtoResponse.Flexibility.Id,
            Description = getFlexibilityDtoResponse.Flexibility.Description,
            Active = getFlexibilityDtoResponse.Flexibility.Active,
            Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new() { Request = Request }).Self
                }
            }
        };

        var flexibilityApiResponse = new FlexibilityApiResponse
        {
            Flexibility = flexibilityApi
        };
        return StatusCode((int)HttpStatusCode.OK, flexibilityApiResponse);
    }
}