using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Valeting.ApiObjects.VehicleSize;

namespace Valeting.Controllers.BaseController
{
    public abstract class VehicleSizeBaseController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [Route("/Valeting/vehicleSizes")]
        [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<VehicleSizeApiPaginatedResponse>))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> ListAllAsync([FromQuery] VehicleSizeApiParameters vehicleSizeApiParameters);

        [HttpGet]
        [Authorize]
        [Route("/Valeting/vehicleSizes/{id}")]
        [ProducesResponseType(statusCode: 200, type: typeof(VehicleSizeApiResponse))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> FindByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);
    }
}
