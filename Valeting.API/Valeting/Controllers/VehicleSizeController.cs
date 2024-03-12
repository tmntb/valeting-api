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

namespace Valeting.Controllers;

public class VehicleSizeController(IRedisCache redisCache, IVehicleSizeService vehicleSizeService, IUrlService urlService) : VehicleSizeBaseController
{
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

            var vehicleSizeListDTO = await redisCache.GetRecordAsync<VehicleSizeListDTO>(recordKey);
            if (vehicleSizeListDTO == null)
            {
                vehicleSizeListDTO = await vehicleSizeService.ListAllAsync(vehicleSizeFilterDTO);
                await redisCache.SetRecordAsync<VehicleSizeListDTO>(recordKey, vehicleSizeListDTO, TimeSpan.FromMinutes(5));
            }

            vehicleSizeApiPaginatedResponse.TotalItems = vehicleSizeListDTO.TotalItems;
            vehicleSizeApiPaginatedResponse.TotalPages = vehicleSizeListDTO.TotalPages;

            var linkDTO = urlService.GeneratePaginatedLinks
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
                    Active = item.Active,
                    Link = new VehicleSizeApiLink()
                    {
                        Self = new LinkApi()
                        {
                            Href = urlService.GenerateSelf(Request.Host.Value, Request.Path.Value, item.Id)
                        }
                    }
                }
                ).ToList()
            );

            return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiPaginatedResponse);
        }
        catch (InputException inputException)
        {
            var vehicleSizeApiError = new VehicleSizeApiError() 
            { 
                Id = Guid.NewGuid(),
                Detail = inputException.Message
            };
            return StatusCode((int)HttpStatusCode.BadRequest, vehicleSizeApiError);
        }
        catch (Exception ex)
        {
            var vehicleSizeApiError = new VehicleSizeApiError() 
            { 
                Id = Guid.NewGuid(),
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, vehicleSizeApiError);
        }
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

            var vehicleSizeDTO = await redisCache.GetRecordAsync<VehicleSizeDTO>(recordKey);
            if(vehicleSizeDTO == null)
            {
                vehicleSizeDTO = await vehicleSizeService.FindByIDAsync(Guid.Parse(id));
                await redisCache.SetRecordAsync<VehicleSizeDTO>(recordKey, vehicleSizeDTO, TimeSpan.FromDays(1));
            }

            var vehicleSizeApi = new VehicleSizeApi()
            {
                Id = vehicleSizeDTO.Id,
                Description = vehicleSizeDTO.Description,
                Active = vehicleSizeDTO.Active,
                Link = new VehicleSizeApiLink()
                {
                    Self = new LinkApi()
                    {
                        Href = urlService.GenerateSelf(Request.Host.Value, Request.Path.HasValue ? Request.Path.Value : string.Empty)
                    }
                }
            };

            vehicleSizeApiResponse.VehicleSize = vehicleSizeApi;

            return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiResponse);
        }
        catch (InputException inputException)
        {
            var vehicleSizeApiError = new VehicleSizeApiError() 
            { 
                Id = Guid.NewGuid(),
                Detail = inputException.Message
            };
            return StatusCode((int)HttpStatusCode.BadRequest, vehicleSizeApiError);
        }
        catch (NotFoundException notFoundException)
        {
            var vehicleSizeApiError = new VehicleSizeApiError() 
            { 
                Id = Guid.NewGuid(),
                Detail = notFoundException.Message
            };
            return StatusCode((int)HttpStatusCode.NotFound, vehicleSizeApiError);
        }
        catch (Exception ex)
        {
            var vehicleSizeApiError = new VehicleSizeApiError() 
            { 
                Id = Guid.NewGuid(),
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, vehicleSizeApiError);
        }
    }
}