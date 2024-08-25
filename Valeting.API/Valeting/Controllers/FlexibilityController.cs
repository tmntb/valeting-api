using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.ComponentModel.DataAnnotations;

using Valeting.Models.Core;
using Valeting.Core.Models.Link;
using Valeting.Models.Flexibility;
using Valeting.Helpers.Interfaces;
using Valeting.Core.Models.Flexibility;
using Valeting.Core.Services.Interfaces;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers;

public class FlexibilityController(IRedisCache redisCache, IFlexibilityService flexibilityService, IUrlService urlService, IMapper mapper) : FlexibilityBaseController
{
    public override async Task<IActionResult> GetAsync([FromQuery] FlexibilityApiParameters flexibilityApiParameters)
    {
        try
        {
            var paginatedFlexibilitySVRequest = mapper.Map<PaginatedFlexibilitySVRequest>(flexibilityApiParameters);

            /*
            var recordKey = string.Format("ListFlexibility_{0}_{1}_{2}", flexibilityApiParameters.PageNumber, flexibilityApiParameters.PageSize, flexibilityApiParameters.Active);
            var paginatedFlexibilitySVResponse = await redisCache.GetRecordAsync<PaginatedFlexibilitySVResponse>(recordKey);
            if (paginatedFlexibilitySVResponse == null)
            {
                paginatedFlexibilitySVResponse = await flexibilityService.GetAsync(paginatedFlexibilitySVRequest);
                if (paginatedFlexibilitySVResponse.HasError)
                {
                    var flexibilityApiError = new FlexibilityApiError()
                    {
                        Detail = paginatedFlexibilitySVResponse.Error.Message
                    };
                    return StatusCode(paginatedFlexibilitySVResponse.Error.ErrorCode, flexibilityApiError);
                }

                await redisCache.SetRecordAsync(recordKey, paginatedFlexibilitySVResponse, TimeSpan.FromMinutes(5));
            }
            */

            var paginatedFlexibilitySVResponse = await flexibilityService.GetAsync(paginatedFlexibilitySVRequest);

            var flexibilityApiPaginatedResponse = new FlexibilityApiPaginatedResponse
            {
                Flexibilities = [],
                CurrentPage = flexibilityApiParameters.PageNumber,
                TotalItems = paginatedFlexibilitySVResponse.TotalItems,
                TotalPages = paginatedFlexibilitySVResponse.TotalPages,
                Links = new()
                {
                    Prev = new() { Href = string.Empty },
                    Next = new() { Href = string.Empty },
                    Self = new() { Href = string.Empty }
                },
            };

            var paginatedLinks = urlService.GeneratePaginatedLinks
            (
                new GeneratePaginatedLinksSVRequest()
                {
                    BaseUrl = Request.Host.Value,
                    Path = Request.Path.HasValue ? Request.Path.Value : string.Empty,
                    QueryString = Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty,
                    PageNumber = flexibilityApiParameters.PageNumber,
                    TotalPages = paginatedFlexibilitySVResponse.TotalPages,
                    Filter = paginatedFlexibilitySVRequest.Filter
                }
            );

            var links = mapper.Map<PaginationLinksApi>(paginatedLinks);
            flexibilityApiPaginatedResponse.Links = links;

            var flexibilityApis = mapper.Map<List<FlexibilityApi>>(paginatedFlexibilitySVResponse.Flexibilities);
            flexibilityApis.ForEach(f => 
                f.Link = new() 
                { 
                    Self = new() 
                    { 
                        Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = Request.Path.Value, Id = f.Id }).Self 
                    } 
                }
            );
            flexibilityApiPaginatedResponse.Flexibilities = flexibilityApis;

            return StatusCode((int)HttpStatusCode.OK, flexibilityApiPaginatedResponse);
        }
        catch (Exception ex)
        {
            var flexibilityApiError = new FlexibilityApiError()
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
            var getFlexibilitySVRequest = new GetFlexibilitySVRequest()
            {
                Id = Guid.Parse(id)
            };

            /*
            var recordKey = string.Format("Flexibility_{0}", id);
            var getFlexibilitySVResponse = await redisCache.GetRecordAsync<GetFlexibilitySVResponse>(recordKey);
            if (getFlexibilitySVResponse == null)
            {
                getFlexibilitySVResponse = await flexibilityService.GetByIdAsync(getFlexibilitySVRequest);
                if (getFlexibilitySVResponse.HasError)
                {
                    var flexibilityApiError = new FlexibilityApiError()
                    {
                        Detail = getFlexibilitySVResponse.Error.Message
                    };
                    return StatusCode(getFlexibilitySVResponse.Error.ErrorCode, flexibilityApiError);
                }

                await redisCache.SetRecordAsync(recordKey, getFlexibilitySVResponse, TimeSpan.FromDays(1));
            }
            */

            var getFlexibilitySVResponse = await flexibilityService.GetByIdAsync(getFlexibilitySVRequest);

            var flexibilityApi = mapper.Map<FlexibilityApi>(getFlexibilitySVResponse.Flexibility);
            flexibilityApi.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = Request.Path.HasValue ? Request.Path.Value : string.Empty }).Self
                }
            };

            var flexibilityApiResponse = new FlexibilityApiResponse()
            {
                Flexibility = flexibilityApi
            };
            return StatusCode((int)HttpStatusCode.OK, flexibilityApiResponse);
        }
        catch (Exception ex)
        {
            var flexibilityApiError = new FlexibilityApiError()
            {
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, flexibilityApiError);
        }
    }
}