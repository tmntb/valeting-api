using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.ComponentModel.DataAnnotations;
using Valeting.Models.Core;
using Valeting.Core.Interfaces;
using Valeting.Cache.Interfaces;
using Valeting.Common.Models.Link;
using Valeting.Models.Flexibility;
using Valeting.Common.Models.Flexibility;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers;

public class FlexibilityController(IFlexibilityService flexibilityService, IUrlService urlService, ICacheHandler cacheHandler, IMapper mapper) : FlexibilityBaseController
{
    public override async Task<IActionResult> GetAsync([FromQuery] FlexibilityApiParameters flexibilityApiParameters)
    {
        try
        {
            var paginatedFlexibilityDtoRequest = mapper.Map<PaginatedFlexibilityDtoRequest>(flexibilityApiParameters);

            var recordKey = string.Format("ListFlexibility_{0}_{1}_{2}", flexibilityApiParameters.PageNumber, flexibilityApiParameters.PageSize, flexibilityApiParameters.Active);
            var paginatedFlexibilityDtoResponse = cacheHandler.GetRecord<PaginatedFlexibilityDtoResponse>(recordKey);
            if (paginatedFlexibilityDtoResponse == null)
            {
                paginatedFlexibilityDtoResponse = await flexibilityService.GetAsync(paginatedFlexibilityDtoRequest);
                if (paginatedFlexibilityDtoResponse.HasError)
                {
                    var flexibilityApiError = new FlexibilityApiError
                    {
                        Detail = paginatedFlexibilityDtoResponse.Error.Message
                    };
                    return StatusCode(paginatedFlexibilityDtoResponse.Error.ErrorCode, flexibilityApiError);
                }

                cacheHandler.SetRecord(recordKey, paginatedFlexibilityDtoResponse, TimeSpan.FromMinutes(5));
            }

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
        catch (Exception ex)
        {
            var flexibilityApiError = new FlexibilityApiError
            {
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, flexibilityApiError);
        }
    }

    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        try
        {
            var getFlexibilityDtoRequest = new GetFlexibilityDtoRequest
            {
                Id = Guid.Parse(id)
            };

            var recordKey = string.Format("Flexibility_{0}", id);
            var getFlexibilityDtoResponse = cacheHandler.GetRecord<GetFlexibilityDtoResponse>(recordKey);
            if (getFlexibilityDtoResponse == null)
            {
                getFlexibilityDtoResponse = await flexibilityService.GetByIdAsync(getFlexibilityDtoRequest);
                if (getFlexibilityDtoResponse.HasError)
                {
                    var flexibilityApiError = new FlexibilityApiError
                    {
                        Detail = getFlexibilityDtoResponse.Error.Message
                    };
                    return StatusCode(getFlexibilityDtoResponse.Error.ErrorCode, flexibilityApiError);
                }

                cacheHandler.SetRecord(recordKey, getFlexibilityDtoResponse, TimeSpan.FromDays(1));
            }

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
        catch (Exception ex)
        {
            var flexibilityApiError = new FlexibilityApiError
            {
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, flexibilityApiError);
        }
    }
}