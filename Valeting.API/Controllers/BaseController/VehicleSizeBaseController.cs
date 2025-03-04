using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Valeting.Models.VehicleSize;

namespace Valeting.Controllers.BaseController;

[Produces("application/json")]
public abstract class VehicleSizeBaseController : ControllerBase
{
    [HttpGet]
    [Authorize]
    [Route("/vehicleSizes")]
    [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<VehicleSizeApiPaginatedResponse>))]
    [ProducesResponseType(statusCode: 400, type: typeof(VehicleSizeApiError))]
    [ProducesResponseType(statusCode: 500, type: typeof(VehicleSizeApiError))]
    public abstract Task<IActionResult> GetAsync([FromQuery] VehicleSizeApiParameters vehicleSizeApiParameters);

    [HttpGet]
    [Authorize]
    [Route("/vehicleSizes/{id}")]
    [ProducesResponseType(statusCode: 200, type: typeof(VehicleSizeApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(VehicleSizeApiError))]
    [ProducesResponseType(statusCode: 404, type: typeof(VehicleSizeApiError))]
    [ProducesResponseType(statusCode: 500, type: typeof(VehicleSizeApiError))]
    public abstract Task<IActionResult> GetByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);
}