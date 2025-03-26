using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Valeting.API.Controllers.BaseController;
using Valeting.API.Models.Core;
using Valeting.API.Models.Flexibility;
using Valeting.Common.Messages;
using Valeting.Common.Models.Flexibility;
using Valeting.Core.Interfaces;

namespace Valeting.API.Controllers;

public class FlexibilityController(IFlexibilityService flexibilityService, IUrlService urlService, IMapper mapper) : FlexibilityBaseController
{
    public override async Task<IActionResult> GetFilteredAsync([FromQuery] FlexibilityApiParameters flexibilityApiParameters)
    {
        ArgumentNullException.ThrowIfNull(flexibilityApiParameters, Messages.InvalidRequestQueryParameters);

        var paginatedFlexibilityDtoRequest = mapper.Map<PaginatedFlexibilityDtoRequest>(flexibilityApiParameters);
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

        var links = mapper.Map<PaginationLinksApi>(paginatedLinks);
        flexibilityApiPaginatedResponse.Links = links;

        var flexibilityApis = mapper.Map<List<FlexibilityApi>>(paginatedFlexibilityDtoResponse.Flexibilities);
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

        var flexibilityApi = mapper.Map<FlexibilityApi>(getFlexibilityDtoResponse.Flexibility);
        flexibilityApi.Link = new()
        {
            Self = new()
            {
                Href = urlService.GenerateSelf(new() { Request = Request }).Self
            }
        };

        var flexibilityApiResponse = new FlexibilityApiResponse
        {
            Flexibility = flexibilityApi
        };
        return StatusCode((int)HttpStatusCode.OK, flexibilityApiResponse);
    }
}