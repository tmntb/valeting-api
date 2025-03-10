using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.ComponentModel.DataAnnotations;
using Valeting.API.Models.Core;
using Valeting.Core.Interfaces;
using Valeting.Common.Models.Link;
using Valeting.API.Models.Flexibility;
using Valeting.Common.Models.Flexibility;
using Valeting.API.Controllers.BaseController;

namespace Valeting.API.Controllers;

public class FlexibilityController(IFlexibilityService flexibilityService, IUrlService urlService, IMapper mapper) : FlexibilityBaseController
{
    public override async Task<IActionResult> GetFilteredAsync([FromQuery] FlexibilityApiParameters flexibilityApiParameters)
    {
        ArgumentNullException.ThrowIfNull(flexibilityApiParameters, "Invalid api parameters");

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
            new GeneratePaginatedLinksDtoRequest
            {
                BaseUrl = Request.Host.Value,
                Path = Request.Path.HasValue ? Request.Path.Value : string.Empty,
                QueryString = Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty,
                PageNumber = flexibilityApiParameters.PageNumber,
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
                    Href = urlService.GenerateSelf(new GenerateSelfUrlDtoRequest { BaseUrl = Request.Host.Value, Path = Request.Path.Value, Id = f.Id }).Self
                }
            }
        );

        flexibilityApiPaginatedResponse.Flexibilities = flexibilityApis;
        return StatusCode((int)HttpStatusCode.OK, flexibilityApiPaginatedResponse);
    }

    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        ArgumentNullException.ThrowIfNull(id, "Invalid request id");

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
                Href = urlService.GenerateSelf(new GenerateSelfUrlDtoRequest { BaseUrl = Request.Host.Value, Path = Request.Path.HasValue ? Request.Path.Value : string.Empty }).Self
            }
        };

        var flexibilityApiResponse = new FlexibilityApiResponse
        {
            Flexibility = flexibilityApi
        };
        return StatusCode((int)HttpStatusCode.OK, flexibilityApiResponse);
    }
}