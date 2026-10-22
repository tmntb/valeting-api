using Api.Models.Auth.Payload;
using Api.Models.Core;
using Api.Models.WebAuthn.Payload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseController;

//TODO: Validar os endpoint se precisa do authorize ou não uma vez que o login vai mudar.

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class AuthBaseController : ControllerBase
{
    /// <summary>
    /// Restore the password for a given user.
    /// </summary>
    /// <param name="forgotPasswordApiRequest">The request containing the user's email.</param>
    /// <response code="200">Indicates that the forgot password process was successfully initiated.</response>
    /// <response code="400">Returned when the request body is invalid or missing required fields.</response>
    /// <response code="409">Returned when the user mfa or recovery code in the request is invalid.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Route("forgot-password")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 409, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordApiRequest forgotPasswordApiRequest);

    /// <summary>
    /// Authenticates a user and generates a JWT access token.
    /// </summary>
    /// <param name="loginApiRequest">The login credentials (username and password) of the user.</param>
    /// <response code="200">Returns a JWT access token and related metadata.</response>
    /// <response code="400">Returned when the request body is invalid or missing required fields.</response>
    /// <response code="401">Returned when authentication fails due to invalid credentials.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(statusCode: 200, type: typeof(LoginApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> LoginAsync([FromBody] LoginApiRequest loginApiRequest);

    /// <summary>
    /// Generates the recovery codes
    /// </summary>
    /// <response code="200">Returns the list of recovery codes.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Authorize]
    [Route("recovery-codes")]
    [ProducesResponseType(statusCode: 200, type: typeof(GenerateRecoveryCodesApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> RecoveryCodesAsync();

    /// <summary>
    /// Refreshes the user JWT access token.
    /// </summary>
    /// <param name="refreshTokenApiRequest">The request containing the current JWT token.</param>
    /// <response code="200">Returns a newly generated JWT token.</response>
    /// <response code="400">Returned when the request body is invalid or missing required fields.</response>
    /// <response code="401">Returned when the token is invalid or expired beyond the refresh window.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Route("refresh-token")]
    [Consumes("application/json")]
    [ProducesResponseType(statusCode: 200, type: typeof(RefreshTokenApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 401, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenApiRequest refreshTokenApiRequest);

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="registerApiRequest">The registration information, including username and password.</param>
    /// <response code="201">Indicates that the user was successfully registered.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="409">Returned when a user with the same username already exists.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Route("register")]
    [ProducesResponseType(statusCode: 201)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 409, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> RegisterAsync([FromBody] RegisterApiRequest registerApiRequest);

    /// <summary>
    /// Lists passkeys belonging to the authenticated user.
    /// </summary>
    /// <response code="200">Returns safe passkey metadata without cryptographic material.</response>
    /// <response code="401">The caller is not authenticated.</response>
    [HttpGet]
    [Authorize]
    [Route("passkeys")]
    [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<PasskeyCredentialApiResponse>))]
    [ProducesResponseType(statusCode: 401, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> PasskeysAsync();

    /// <summary>
    /// Renames a passkey belonging to the authenticated user.
    /// </summary>
    /// <param name="id">The API identifier of the passkey.</param>
    /// <param name="request">The new friendly name.</param>
    /// <response code="204">The passkey was renamed.</response>
    /// <response code="400">The name is invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="404">The passkey does not belong to the caller or does not exist.</response>
    [HttpPut]
    [Authorize]
    [Route("passkeys/{id:guid}")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 401, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> RenamePasskeyAsync([FromRoute] Guid id, [FromBody] PasskeyRenameApiRequest request);

    /// <summary>
    /// Removes a passkey belonging to the authenticated user.
    /// </summary>
    /// <param name="id">The API identifier of the passkey.</param>
    /// <response code="204">The passkey was removed.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="404">The passkey does not belong to the caller or does not exist.</response>
    [HttpDelete]
    [Authorize]
    [Route("passkeys/{id:guid}")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 401, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> RemovePasskeyAsync([FromRoute] Guid id);
}
