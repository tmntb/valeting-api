using Valeting.ApiObjects.User;

using Microsoft.AspNetCore.Mvc;

namespace Valeting.Controllers.BaseController;

[Produces("application/json")]
public abstract class UserBaseController : ControllerBase
{
    [HttpPost]
    [Route("/user/verify")]
    [ProducesResponseType(statusCode: 200, type: typeof(ValidateLoginResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(UserApiError))]
    [ProducesResponseType(statusCode: 404, type: typeof(UserApiError))]
    [ProducesResponseType(statusCode: 500, type: typeof(UserApiError))]
    public abstract Task<IActionResult> ValidateLogin([FromBody] ValidateLoginRequest validateLoginRequest);
}