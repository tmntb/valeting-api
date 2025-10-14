using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Api.Models.Core;
using Api.Models.Flexibility;

namespace Api.Controllers.BaseController;

[Produces("application/json")]
public abstract class FlexibilityBaseController : ControllerBase
{
    [HttpGet]
    [Authorize]
    [Route("/flexibilities")]
    [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<FlexibilityApiPaginatedResponse>))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> GetFilteredAsync([FromQuery] FlexibilityApiParameters flexibilityApiParameters);

    [HttpGet]
    [Authorize]
    [Route("/flexibilities/{id}")]
    [ProducesResponseType(statusCode: 200, type: typeof(FlexibilityApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> GetByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);
}