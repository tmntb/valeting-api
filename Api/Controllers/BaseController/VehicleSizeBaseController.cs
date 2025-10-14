using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Api.Models.Core;
using Api.Models.VehicleSize;

namespace Api.Controllers.BaseController;

[Produces("application/json")]
public abstract class VehicleSizeBaseController : ControllerBase
{
    [HttpGet]
    [Authorize]
    [Route("/vehicleSizes")]
    [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<VehicleSizeApiPaginatedResponse>))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> GetFilteredAsync([FromQuery] VehicleSizeApiParameters vehicleSizeApiParameters);

    [HttpGet]
    [Authorize]
    [Route("/vehicleSizes/{id}")]
    [ProducesResponseType(statusCode: 200, type: typeof(VehicleSizeApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> GetByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);
}