using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.ApiObjects;
using Valeting.Services.Interfaces;
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
                var vehicleSize = await _vehicleSizeService.FindByIDAsync(Guid.Parse(id));

                var vehicleSizeApi = new VehicleSizeApi()
                {
                    Id = vehicleSize.Id,
                    Description = vehicleSize.Description
                };

                return StatusCode((int)HttpStatusCode.OK, vehicleSizeApi);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public override async Task<IActionResult> ListAllAsync()
        {
            try
            {
                var vehicleSizeApis = new List<VehicleSizeApi>();

                var vehicleSizes = await _vehicleSizeService.ListAllAsync();

                vehicleSizeApis.AddRange(
                    vehicleSizes.Select(item => new VehicleSizeApi()
                    {
                        Id = item.Id,
                        Description = item.Description
                    })
                );

                return StatusCode((int)HttpStatusCode.OK, vehicleSizeApis);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
