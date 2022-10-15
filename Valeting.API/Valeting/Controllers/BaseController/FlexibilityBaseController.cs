using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Valeting.ApiObjects;
using Valeting.ApiObjects.Flexibility;

namespace Valeting.Controllers.BaseController
{
    public abstract class FlexibilityBaseController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [Route("/Valeting/flexibilities")]
        [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<FlexibilityApiPaginatedResponse>))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> ListAllAsync([FromQuery] FlexibilityApiParameters flexibilityApiParameters);

        [HttpGet]
        [Authorize]
        [Route("/Valeting/flexibilities/{id}")]
        [ProducesResponseType(statusCode: 200, type: typeof(FlexibilityApiResponse))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> FindByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);
    }
}
