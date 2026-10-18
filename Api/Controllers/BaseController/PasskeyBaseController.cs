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

    /// <summary>
    /// Verifies and stores a WebAuthn registration response for the authenticated user.
    /// </summary>
    /// <param name="request">The ceremony ID, attestation response, and optional friendly name.</param>
    /// <response code="204">The credential was verified and registered.</response>
    /// <response code="400">The request body is invalid.</response>
    /// <response code="401">The ceremony is invalid, expired, or belongs to another user.</response>
    /// <response code="409">The credential is already registered.</response>
    /// <response code="500">An unexpected error occurred.</response>
    [HttpPost]
    [Authorize]
    [Route("register/verify")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 401, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 409, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> PasskeyRegisterVerifyAsync([FromBody] PasskeyRegisterVerifyApiRequest request);
}
