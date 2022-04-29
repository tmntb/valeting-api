using DJValeting.ApiObjects;

using Microsoft.AspNetCore.Mvc;

namespace DJValeting.Controllers.BaseController
{
    public abstract class UserBaseController : ControllerBase
    {
        [HttpPost]
        [Route("/DJValeting/users/verify/")]
        [ProducesResponseType(statusCode: 200, type: typeof(bool))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> ValidateLogin([FromBody] UserApi userApi);
    }
}
