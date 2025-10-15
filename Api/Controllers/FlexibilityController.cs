using Api.Controllers.BaseController;
using Api.Models.Core;
using Api.Models.Flexibility;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.Flexibility.Payload;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Api.Controllers;

public class FlexibilityController(IFlexibilityService flexibilityService, ILinkService urlService) : FlexibilityBaseController
{
    public override async Task<IActionResult> GetFilteredAsync([FromQuery] FlexibilityApiParameters flexibilityApiParameters)
    {
        ArgumentNullException.ThrowIfNull(flexibilityApiParameters, Messages.InvalidRequestQueryParameters);

        var flexibilityFilterDto = new FlexibilityFilterDto 
        { 
                PageNumber = flexibilityApiParameters.PageNumber,
                PageSize = flexibilityApiParameters.PageSize,
                Active = flexibilityApiParameters.Active
        };
        var paginatedFlexibilityDtoResponse = await flexibilityService.GetFilteredAsync(flexibilityFilterDto);

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
                Filter = flexibilityFilterDto
            }
        );

        var links = new PaginationLinksApi 
        {
            Self = new() { Href = paginatedLinks.Self },
            Next = new() { Href = paginatedLinks.Next },
            Prev = new() { Href = paginatedLinks.Prev }
        };
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
                    Href = urlService.GenerateSelf(new() { Request = Request, Id = f.Id })
                }
            }
        );

        flexibilityApiPaginatedResponse.Flexibilities = flexibilityApis;
        return StatusCode((int)HttpStatusCode.OK, flexibilityApiPaginatedResponse);
    }

    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        ArgumentNullException.ThrowIfNull(id, Messages.InvalidRequestId);

        var flexibilityDto = await flexibilityService.GetByIdAsync(Guid.Parse(id));

        var flexibilityApi = new FlexibilityApi
        {
            Id = flexibilityDto.Id,
            Description = flexibilityDto.Description,
            Active = flexibilityDto.Active,
            Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new() { Request = Request })
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