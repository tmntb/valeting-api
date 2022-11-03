using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.ApiObjects;
using Valeting.Common.Exceptions;
using Valeting.Helpers.Interfaces;
using Valeting.Services.Interfaces;
using Valeting.Business.VehicleSize;
using Valeting.ApiObjects.VehicleSize;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers
{
    public class VehicleSizeController : VehicleSizeBaseController
    {
        private IRedisCache _redisCache;
        private readonly IUrlService _urlService;
        private readonly IVehicleSizeService _vehicleSizeService;
        private VehicleSizeApiError _vehicleSizeApiError;

        public VehicleSizeController(IRedisCache redisCache, IVehicleSizeService vehicleSizeService, IUrlService urlService)
        {
            _redisCache = redisCache;
            _vehicleSizeService = vehicleSizeService;
            _urlService = urlService;
            _vehicleSizeApiError = new VehicleSizeApiError() { Id = Guid.NewGuid() };
        }

        public override async Task<IActionResult> FindByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
        {
            try
            {
                var vehicleSizeApiResponse = new VehicleSizeApiResponse()
                {
                    VehicleSize =  new VehicleSizeApi()
                };

                var recordKey = string.Format("VehicleSize_{0}", id);

                var vehicleSizeDTO = await _redisCache.GetRecordAsync<VehicleSizeDTO>(recordKey);
                if(vehicleSizeDTO == null)
                {
                    vehicleSizeDTO = await _vehicleSizeService.FindByIDAsync(Guid.Parse(id));
                    await _redisCache.SetRecordAsync<VehicleSizeDTO>(recordKey, vehicleSizeDTO, TimeSpan.FromDays(1));
                }

                var vehicleSizeApi = new VehicleSizeApi()
                {
                    Id = vehicleSizeDTO.Id,
                    Description = vehicleSizeDTO.Description,
                    Actice = vehicleSizeDTO.Active,
                    Link = new VehicleSizeApiLink()
                    {
                        Self = new LinkApi()
                        {
                            Href = _urlService.GenerateSelf(Request.Host.Value, Request.Path.HasValue ? Request.Path.Value : string.Empty)
                        }
                    }
                };

                vehicleSizeApiResponse.VehicleSize = vehicleSizeApi;

                return StatusCode((int)HttpStatusCode.OK, vehicleSizeApi);
            }
            catch (InputException inputException)
            {
                _vehicleSizeApiError.Detail = inputException.Message;
                return StatusCode((int)HttpStatusCode.BadRequest, _vehicleSizeApiError);
            }
            catch (NotFoundException notFoundException)
            {
                _vehicleSizeApiError.Detail = notFoundException.Message;
                return StatusCode((int)HttpStatusCode.NotFound, _vehicleSizeApiError);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, _vehicleSizeApiError);
            }
        }

        public override async Task<IActionResult> ListAllAsync([FromQuery] VehicleSizeApiParameters vehicleSizeApiParameters)
        {
            try
            {
                var vehicleSizeApiPaginatedResponse = new VehicleSizeApiPaginatedResponse()
                {
                    VehicleSizes = new List<VehicleSizeApi>(),
                    CurrentPage = vehicleSizeApiParameters.PageNumber,
                    Links = new PaginationLinksApi()
                    {
                        Prev = new LinkApi() { Href = string.Empty },
                        Next = new LinkApi() { Href = string.Empty },
                        Self = new LinkApi() { Href = string.Empty }
                    }
                };

                var vehicleSizeFilterDTO = new VehicleSizeFilterDTO()
                {
                    PageNumber = vehicleSizeApiParameters.PageNumber,
                    PageSize = vehicleSizeApiParameters.PageSize,
                    Active = vehicleSizeApiParameters.Active
                };

                var recordKey = string.Format("ListVehicleSize_{0}_{1}_{2}", vehicleSizeFilterDTO.PageNumber, vehicleSizeFilterDTO.PageSize, vehicleSizeFilterDTO.Active);

                var vehicleSizeListDTO = await _redisCache.GetRecordAsync<VehicleSizeListDTO>(recordKey);
                if(vehicleSizeListDTO == null)
                {
                    vehicleSizeListDTO = await _vehicleSizeService.ListAllAsync(vehicleSizeFilterDTO);
                    await _redisCache.SetRecordAsync<VehicleSizeListDTO>(recordKey, vehicleSizeListDTO, TimeSpan.FromMinutes(5));
                }

                vehicleSizeApiPaginatedResponse.TotalItems = vehicleSizeListDTO.TotalItems;
                vehicleSizeApiPaginatedResponse.TotalPages = vehicleSizeListDTO.TotalPages;

                var linkDTO = _urlService.GeneratePaginatedLinks
                (
                    Request.Host.Value,
                    Request.Path.HasValue ? Request.Path.Value : string.Empty,
                    Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty,
                    vehicleSizeApiParameters.PageNumber, vehicleSizeListDTO.TotalPages, vehicleSizeFilterDTO
                );

                vehicleSizeApiPaginatedResponse.Links.Prev.Href = linkDTO.Prev;
                vehicleSizeApiPaginatedResponse.Links.Next.Href = linkDTO.Next;
                vehicleSizeApiPaginatedResponse.Links.Self.Href = linkDTO.Self;

                vehicleSizeApiPaginatedResponse.VehicleSizes.AddRange(
                    vehicleSizeListDTO.VehicleSizes.Select(item => new VehicleSizeApi()
                        {
                            Id = item.Id,
                            Description = item.Description,
                            Actice = item.Active,
                            Link = new VehicleSizeApiLink()
                            {
                                Self = new LinkApi()
                                {
                                    Href = _urlService.GenerateSelf(Request.Host.Value, Request.Path.Value, item.Id)
                                }
                            }
                        }
                    ).ToList()
                );

                return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiPaginatedResponse);
            }
            catch (InputException inputException)
            {
                _vehicleSizeApiError.Detail = inputException.Message;
                return StatusCode((int)HttpStatusCode.BadRequest, _vehicleSizeApiError);
            }
            catch (Exception ex)
            {
                _vehicleSizeApiError.Detail = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, _vehicleSizeApiError);
            }
        }
    }
}
