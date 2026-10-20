using Api.Models.Auth.Payload;
using Api.Models.Core;
using Api.Models.WebAuthn.Payload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseController;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class SessionBaseController : ControllerBase
{
    /// <summary>
    /// Setups the mfa for a given user
    /// </summary>
    /// <response code="200">Return the mfa info required for the setup.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="409">Returned when the user mfa is already activated.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Authorize]
    [Route("mfa/setup")]
    [ProducesResponseType(statusCode: 200, type: typeof(MfaSetupApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 409, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> MfaSetupAsync();

    /// <summary>
    /// Enables the mfa for a given user
    /// </summary>
    /// <param name="mfaCodeApiRequest">The mfa code information</param>
    /// <response code="204">Indicates that the mfa is enabled.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="409">Returned when the user mfa is already activated.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Authorize]
    [Route("mfa/verify")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 409, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> MfaVerifyAsync([FromBody] MfaCodeApiRequest mfaCodeApiRequest);
    
    /// <summary>
    /// Creates WebAuthn assertion options for discoverable or email-directed passkey sign-in.
    /// </summary>
    /// <param name="webAuthnAuthenticateOptionsApiRequest">Email to limit the eligible webAuthn.</param>
    /// <response code="200">Returns the browser assertion options and a one-time ceremony ID.</response>
    /// <response code="400">The request body is invalid.</response>
    /// <response code="500">An unexpected error occurred.</response>
    [HttpPost]
    [Route("webauthn/authenticate/options")]
    [ProducesResponseType(statusCode: 200, type: typeof(WebAuthnAuthenticateOptionsApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> WebAuthnAuthenticateOptionsAsync([FromBody] WebAuthnAuthenticateOptionsApiRequest webAuthnAuthenticateOptionsApiRequest);
    
    /// <summary>
    /// Creates WebAuthn registration options for the authenticated user.
    /// </summary>
    /// <response code="200">Returns the browser registration options and a one-time ceremony ID.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="500">An unexpected error occurred.</response>
    [HttpPost]
    [Authorize]
    [Route("webauthn/register/options")]
    [ProducesResponseType(statusCode: 200, type: typeof(WebauthnRegisterOptionsApiResponse))]
    [ProducesResponseType(statusCode: 401, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> WebAuthnRegisterOptionsAsync();

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
    [Route("webauthn/register/verify")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 401, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 409, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> WebAuthnRegisterVerifyAsync([FromBody] WebauthnRegisterVerifyApiRequest request);
}
