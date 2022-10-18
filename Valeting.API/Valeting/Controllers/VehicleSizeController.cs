using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.ApiObjects;
using Valeting.Services.Interfaces;
using Valeting.Business.VehicleSize;
using Valeting.ApiObjects.VehicleSize;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers
{
    public class VehicleSizeController : VehicleSizeBaseController
    {
        private readonly IVehicleSizeService _vehicleSizeService;

        public VehicleSizeController(IVehicleSizeService vehicleSizeService)
        {
            _vehicleSizeService = vehicleSizeService;
        }

        public override async Task<IActionResult> FindByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
        {
            try
            {
                var vehicleSizeApiResponse = new VehicleSizeApiResponse()
                {
                    VehicleSize =  new VehicleSizeApi()
                };

                var vehicleSizeDTO = await _vehicleSizeService.FindByIDAsync(Guid.Parse(id));

                var vehicleSizeApi = new VehicleSizeApi()
                {
                    Id = vehicleSizeDTO.Id,
                    Description = vehicleSizeDTO.Description,
                    Actice = vehicleSizeDTO.Active,
                    Link = new VehicleSizeApiLink() { Self = new LinkApi() { Href = "" } }
                };

                vehicleSizeApiResponse.VehicleSize = vehicleSizeApi;

                return StatusCode((int)HttpStatusCode.OK, vehicleSizeApi);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
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
                };

                var vehicleSizeFilterDTO = new VehicleSizeFilterDTO()
                {
                    PageNumber = vehicleSizeApiParameters.PageNumber,
                    PageSize = vehicleSizeApiParameters.PageSize,
                    Active = vehicleSizeApiParameters.Active
                };

                var vehicleSizeListDTO = await _vehicleSizeService.ListAllAsync(vehicleSizeFilterDTO);

                vehicleSizeApiPaginatedResponse.TotalItems = vehicleSizeListDTO.TotalItems;
                vehicleSizeApiPaginatedResponse.TotalPages = vehicleSizeListDTO.TotalPages;

                vehicleSizeApiPaginatedResponse.VehicleSizes.AddRange(
                    vehicleSizeListDTO.VehicleSizes.Select(item => new VehicleSizeApi()
                        {
                            Id = item.Id,
                            Description = item.Description,
                            Actice = item.Active,
                            Link = new VehicleSizeApiLink()
                        }
                    ).ToList()
                );

                return StatusCode((int)HttpStatusCode.OK, vehicleSizeApiPaginatedResponse);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
