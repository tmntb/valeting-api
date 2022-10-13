using Valeting.ApiObjects.User;

using Microsoft.AspNetCore.Mvc;

namespace Valeting.Controllers.BaseController
{
    [ApiController]
    public abstract class UserBaseController : ControllerBase
    {
        [HttpPost]
        [Route("/Valeting/users/verify")]
        [ProducesResponseType(statusCode: 200, type: typeof(ValidateLoginResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(UserApiError))]
        public abstract Task<IActionResult> ValidateLogin([FromBody] ValidateLoginRequest validateLoginRequest);
    }
}
