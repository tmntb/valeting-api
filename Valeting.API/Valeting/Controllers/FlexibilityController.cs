﻿using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.Helpers;
using Valeting.ApiObjects;
using Valeting.Helpers.Interfaces;
using Valeting.Services.Interfaces;
using Valeting.Business.Flexibility;
using Valeting.ApiObjects.Flexibility;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers
{
    public class FlexibilityController : FlexibilityBaseController
    {
        private readonly IFlexibilityService _flexibilityService;
        private IRedisCache _redisCache;

        public FlexibilityController(IRedisCache redisCache, IFlexibilityService flexibilityService)
        {
            _redisCache = redisCache;
            _flexibilityService = flexibilityService;
        }

        public override async Task<IActionResult> FindByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
        {
            try
            {
                var flexibilityApiResponse = new FlexibilityApiResponse()
                {
                    Flexibility = new FlexibilityApi()
                };

                var recordKey = string.Format("Flexibility_{0}", id);

                var flexibilityDTO = await _redisCache.GetRecordAsync<FlexibilityDTO>(recordKey);
                if(flexibilityDTO == null)
                {
                    flexibilityDTO = await _flexibilityService.FindByIDAsync(Guid.Parse(id));
                    await _redisCache.SetRecordAsync<FlexibilityDTO>(recordKey, flexibilityDTO, TimeSpan.FromDays(1));
                }

                var flexibilityApi = new FlexibilityApi()
                {
                    Id = flexibilityDTO.Id,
                    Description = flexibilityDTO.Description,
                    Active = flexibilityDTO.Active,
                    Link = new FlexibilityApiLink() { Self = new LinkApi() { Href = "" } }
                };

                flexibilityApiResponse.Flexibility = flexibilityApi;

                return StatusCode((int)HttpStatusCode.OK, flexibilityApiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public override async Task<IActionResult> ListAllAsync([FromQuery] FlexibilityApiParameters flexibilityApiParameters)
        {
            try
            {
                var flexibilityApiPaginatedResponse = new FlexibilityApiPaginatedResponse()
                {
                    Flexibilities = new List<FlexibilityApi>(),
                    CurrentPage = flexibilityApiParameters.PageNumber,
                    Links = new PaginationLinksApi() { Prev = new LinkApi(), Self = new LinkApi(), Next = new LinkApi() }
                };

                var flexibilityFilterDTO = new FlexibilityFilterDTO()
                {
                    PageNumber = flexibilityApiParameters.PageNumber,
                    PageSize = flexibilityApiParameters.PageSize,
                    Active = flexibilityApiParameters.Active
                };

                var recordKey = string.Format("ListFlexibility_{0}_{1}_{2}", flexibilityFilterDTO.PageNumber, flexibilityFilterDTO.PageSize, flexibilityFilterDTO.Active);

                var flexibilityListDTO = await _redisCache.GetRecordAsync<FlexibilityListDTO>(recordKey);
                if(flexibilityListDTO == null)
                {
                    flexibilityListDTO = await _flexibilityService.ListAllAsync(flexibilityFilterDTO);
                    await _redisCache.SetRecordAsync<FlexibilityListDTO>(recordKey, flexibilityListDTO, TimeSpan.FromMinutes(5));
                }

                flexibilityApiPaginatedResponse.TotalItems = flexibilityListDTO.TotalItems;
                flexibilityApiPaginatedResponse.TotalPages = flexibilityListDTO.TotalPages;

                flexibilityApiPaginatedResponse.Flexibilities.AddRange(
                    flexibilityListDTO.Flexibilities.Select(item => new FlexibilityApi()
                        {
                            Id = item.Id,
                            Description = item.Description,
                            Active = item.Active,
                            Link = new FlexibilityApiLink() { Self = new LinkApi() { Href = "https://examplehost/exampleapi/v1/example-resource/1" } } //por fazer
                        }
                    ).ToList()
                );

                return StatusCode((int)HttpStatusCode.OK, flexibilityApiPaginatedResponse);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
