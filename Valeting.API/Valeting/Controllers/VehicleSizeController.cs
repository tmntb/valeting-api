using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.Service;
using Valeting.Business;
using Valeting.ApiObjects;
using Valeting.Repositories;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers
{
    public class VehicleSizeController : VehicleSizeBaseController
    {
        private readonly IConfiguration _configuration;

        public VehicleSizeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override async Task<IActionResult> FindByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
        {
            try
            {
                VehicleSizeService vehicleSizeService = new(new VehicleSizeRepository(_configuration));
                VehicleSizeDTO vehicleSize = await vehicleSizeService.FindByIDAsync(Guid.Parse(id));

                VehicleSizeApi vehicleSizeApi = new()
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
                List<VehicleSizeApi> vehicleSizeApis = new();

                VehicleSizeService vehicleSizeService = new(new VehicleSizeRepository(_configuration));
                IEnumerable<VehicleSizeDTO> vehicleSizes = await vehicleSizeService.ListAllAsync();

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
