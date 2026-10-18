using Api.Models.Core;
using Api.Models.Passkey.Payload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseController;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class PasskeyBaseController : ControllerBase
{
    /// <summary>
    /// Creates WebAuthn registration options for the authenticated user.
    /// </summary>
    /// <response code="200">Returns the browser registration options and a one-time ceremony ID.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="500">An unexpected error occurred.</response>
    [HttpPost]
    [Authorize]
    [Route("register/options")]
    [ProducesResponseType(statusCode: 200, type: typeof(PasskeyRegisterOptionsApiResponse))]
    [ProducesResponseType(statusCode: 401, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> PasskeyRegisterOptionsAsync();
}
