using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.Helpers.Interfaces;
using Valeting.Services.Interfaces;
using Valeting.ApiObjects.Flexibility;
using Valeting.Controllers.BaseController;
using Valeting.Services.Objects.Flexibility;

namespace Valeting.Controllers;

public class FlexibilityController(IRedisCache redisCache, IFlexibilityService flexibilityService, IUrlService urlService) : FlexibilityBaseController
{
    public override async Task<IActionResult> FindByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        try
        {
            var getFlexibilitySVRequest = new GetFlexibilitySVRequest()
            {
                Id = Guid.Parse(id)
            };

            var recordKey = string.Format("Flexibility_{0}", id);
            var getFlexibilitySVResponse = await redisCache.GetRecordAsync<GetFlexibilitySVResponse>(recordKey);
            if(getFlexibilitySVResponse == null)
            {
                getFlexibilitySVResponse = await flexibilityService.GetAsync(getFlexibilitySVRequest);
                if(getFlexibilitySVResponse.HasError)
                {
                    var flexibilityApiError = new FlexibilityApiError() 
                    { 
                        Detail = getFlexibilitySVResponse.Error.Message
                    };
                    return StatusCode(getFlexibilitySVResponse.Error.ErrorCode, flexibilityApiError);
                }

                await redisCache.SetRecordAsync(recordKey, getFlexibilitySVResponse, TimeSpan.FromDays(1));
            }

            var flexibilityApiResponse = new FlexibilityApiResponse()
            {
                Flexibility = new()
                {
                    Id = getFlexibilitySVResponse.Id,
                    Description = getFlexibilitySVResponse.Description,
                    Active = getFlexibilitySVResponse.Active,
                    Link = new()
                    {
                        Self = new()
                        {
                            Href = urlService.GenerateSelf(Request.Host.Value, Request.Path.HasValue ? Request.Path.Value : string.Empty)
                        }
                    }
                }
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

    public override async Task<IActionResult> ListAllAsync([FromQuery] FlexibilityApiParameters flexibilityApiParameters)
    {
        try
        {
            var paginatedFlexibilitySVRequest = new PaginatedFlexibilitySVRequest()
            {
                Filter = new()
                {
                    PageNumber = flexibilityApiParameters.PageNumber,
                    PageSize = flexibilityApiParameters.PageSize,
                    Active = flexibilityApiParameters.Active
                }
            };

            var recordKey = string.Format("ListFlexibility_{0}_{1}_{2}", flexibilityApiParameters.PageNumber, flexibilityApiParameters.PageSize, flexibilityApiParameters.Active);
            var paginatedFlexibilitySVResponse = await redisCache.GetRecordAsync<PaginatedFlexibilitySVResponse>(recordKey);
            if (paginatedFlexibilitySVResponse == null)
            {
                paginatedFlexibilitySVResponse = await flexibilityService.ListAllAsync(paginatedFlexibilitySVRequest);
                if(paginatedFlexibilitySVResponse.HasError)
                {
                    var flexibilityApiError = new FlexibilityApiError()
                    {
                        Detail = paginatedFlexibilitySVResponse.Error.Message
                    };
                    return StatusCode(paginatedFlexibilitySVResponse.Error.ErrorCode, flexibilityApiError);
                }
                
                await redisCache.SetRecordAsync(recordKey, paginatedFlexibilitySVResponse, TimeSpan.FromMinutes(5));
            }

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

            var linkDTO = urlService.GeneratePaginatedLinks
            (
                Request.Host.Value,
                Request.Path.HasValue ? Request.Path.Value : string.Empty,
                Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty,
                flexibilityApiParameters.PageNumber, 
                paginatedFlexibilitySVResponse.TotalPages, 
                paginatedFlexibilitySVRequest.Filter
            );

            flexibilityApiPaginatedResponse.Links.Prev.Href = linkDTO.Prev;
            flexibilityApiPaginatedResponse.Links.Next.Href = linkDTO.Next;
            flexibilityApiPaginatedResponse.Links.Self.Href = linkDTO.Self;

            flexibilityApiPaginatedResponse.Flexibilities.AddRange(
                paginatedFlexibilitySVResponse.Flexibilities.Select(item => 
                    new FlexibilityApi()
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Active = item.Active,
                        Link = new()
                        {
                            Self = new()
                            {
                                Href = urlService.GenerateSelf(Request.Host.Value, Request.Path.Value, item.Id)
                            }
                        }
                    }
                ).ToList()
            );
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
}