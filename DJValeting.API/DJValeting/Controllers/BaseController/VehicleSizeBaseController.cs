using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using DJValeting.ApiObjects;

namespace DJValeting.Controllers.BaseController
{
    public abstract class VehicleSizeBaseController : ControllerBase
    {
        [HttpGet]
        [Route("/DJValeting/vehicleSizes")]
        [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<VehicleSizeApi>))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> ListAllAsync();

        [HttpGet]
        [Route("/DJValeting/vehicleSizes/{id}")]
        [ProducesResponseType(statusCode: 200, type: typeof(VehicleSizeApi))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> FindByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);
    }
}
