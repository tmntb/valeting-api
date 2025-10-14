using Microsoft.AspNetCore.Mvc;
using Api.Models.Core;
using Api.Models.User;

namespace Api.Controllers.BaseController;

[Produces("application/json")]
public abstract class UserBaseController : ControllerBase
{
    [HttpPost]
    [Route("/user/login")]
    [ProducesResponseType(statusCode: 200, type: typeof(LoginApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> Login([FromBody] LoginApiRequest loginApiRequest);

    [HttpPost]
    [Route("/user/register")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> Register([FromBody] RegisterApiRequest registerApiRequest);
}