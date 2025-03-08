using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Valeting.API.Models.Flexibility;

namespace Valeting.API.Controllers.BaseController;

[Produces("application/json")]
public abstract class FlexibilityBaseController : ControllerBase
{
    [HttpGet]
    [Authorize]
    [Route("/flexibilities")]
    [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<FlexibilityApiPaginatedResponse>))]
    [ProducesResponseType(statusCode: 400, type: typeof(FlexibilityApiError))]
    [ProducesResponseType(statusCode: 500, type: typeof(FlexibilityApiError))]
    public abstract Task<IActionResult> GetAsync([FromQuery] FlexibilityApiParameters flexibilityApiParameters);

    [HttpGet]
    [Authorize]
    [Route("/flexibilities/{id}")]
    [ProducesResponseType(statusCode: 200, type: typeof(FlexibilityApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(FlexibilityApiError))]
    [ProducesResponseType(statusCode: 404, type: typeof(FlexibilityApiError))]
    [ProducesResponseType(statusCode: 500, type: typeof(FlexibilityApiError))]
    public abstract Task<IActionResult> GetByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);
}