using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using DJValeting.ApiObjects;

namespace DJValeting.Controllers.BaseController
{
    public abstract class FlexibilityBaseController : ControllerBase
    {
        [HttpGet]
        [Route("/DJValeting/flexibilities")]
        [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<FlexibilityApi>))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> ListAllAsync();

        [HttpGet]
        [Route("/DJValeting/flexibilities/{id}")]
        [ProducesResponseType(statusCode: 200, type: typeof(FlexibilityApi))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> FindByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);
    }
}
