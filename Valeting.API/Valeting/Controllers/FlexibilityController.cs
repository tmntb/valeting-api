using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.ApiObjects;
using Valeting.Common.Exceptions;
using Valeting.Helpers.Interfaces;
using Valeting.Services.Interfaces;
using Valeting.Business.Flexibility;
using Valeting.ApiObjects.Flexibility;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers
{
    public class FlexibilityController : FlexibilityBaseController
    {
        private IRedisCache _redisCache;
        private readonly IUrlService _urlService;
        private readonly IFlexibilityService _flexibilityService;
        private FlexibilityApiError _flexibilityApiError;

        public FlexibilityController(IRedisCache redisCache, IFlexibilityService flexibilityService, IUrlService urlService)
        {
            _redisCache = redisCache;
            _flexibilityService = flexibilityService;
            _urlService = urlService;
            _flexibilityApiError = new FlexibilityApiError() { Id = Guid.NewGuid() };
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
                    Link = new FlexibilityApiLink()
                    {
                        Self = new LinkApi()
                        {
                            Href = _urlService.GenerateSelf(Request.Host.Value, Request.Path.HasValue ? Request.Path.Value : string.Empty)
                        }
                    }
                };

                flexibilityApiResponse.Flexibility = flexibilityApi;

                return StatusCode((int)HttpStatusCode.OK, flexibilityApiResponse);
            }
            catch (InputException inputException)
            {
                _flexibilityApiError.Detail = inputException.Message;
                return StatusCode((int)HttpStatusCode.BadRequest, _flexibilityApiError);
            }
            catch (NotFoundException notFoundException)
            {
                _flexibilityApiError.Detail = notFoundException.Message;
                return StatusCode((int)HttpStatusCode.NotFound, _flexibilityApiError);
            }
            catch (Exception ex)
            {
                _flexibilityApiError.Detail = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, _flexibilityApiError);
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
                    Links = new PaginationLinksApi()
                    {
                        Prev = new LinkApi() { Href = string.Empty },
                        Next = new LinkApi() { Href = string.Empty },
                        Self = new LinkApi() { Href = string.Empty }
                    }
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

                var linkDTO = _urlService.GeneratePaginatedLinks
                (
                    Request.Host.Value,
                    Request.Path.HasValue ? Request.Path.Value : string.Empty,
                    Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty,
                    flexibilityApiParameters.PageNumber, flexibilityListDTO.TotalPages, flexibilityFilterDTO
                );

                flexibilityApiPaginatedResponse.Links.Prev.Href = linkDTO.Prev;
                flexibilityApiPaginatedResponse.Links.Next.Href = linkDTO.Next;
                flexibilityApiPaginatedResponse.Links.Self.Href = linkDTO.Self;

                flexibilityApiPaginatedResponse.Flexibilities.AddRange(
                    flexibilityListDTO.Flexibilities.Select(item => new FlexibilityApi()
                        {
                            Id = item.Id,
                            Description = item.Description,
                            Active = item.Active,
                            Link = new FlexibilityApiLink()
                            {
                                Self = new LinkApi()
                                {
                                    Href = _urlService.GenerateSelf(Request.Host.Value, Request.Path.Value, item.Id)
                                }
                            }
                        }
                    ).ToList()
                );

                return StatusCode((int)HttpStatusCode.OK, flexibilityApiPaginatedResponse);
            }
            catch (InputException inputException)
            {
                _flexibilityApiError.Detail = inputException.Message;
                return StatusCode((int)HttpStatusCode.BadRequest, _flexibilityApiError);
            }
            catch (Exception ex)
            {
                _flexibilityApiError.Detail = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, _flexibilityApiError);
            }
        }
    }
}
