using Valeting.ApiObjects;

using Microsoft.AspNetCore.Mvc;

namespace Valeting.Controllers.BaseController
{
    public abstract class UserBaseController : ControllerBase
    {
        [HttpPost]
        [Route("/Valeting/users/verify/")]
        [ProducesResponseType(statusCode: 200, type: typeof(bool))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> ValidateLogin([FromBody] UserApi userApi);
    }
}
