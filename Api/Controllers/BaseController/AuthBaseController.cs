using Api.Models.Auth.Payload;
using Api.Models.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseController;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class AuthBaseController : ControllerBase
{
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
    [Route("mfa/enable")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 409, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> MfaEnableAsync([FromBody] MfaCodeApiRequest mfaCodeApiRequest);

    /// <summary>
    /// Regenerates the recovery codes
    /// </summary>
    /// <param name="mfaCodeApiRequest">The information of mfa code to allow the recovery codes to be regenerated</param>
    /// <response code="200">Returns the list of recovery codes.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Authorize]
    [Route("mfa/recovery-codes/regenerate")]
    [ProducesResponseType(statusCode: 200, type: typeof(MfaRegenerateRecoveryCodesApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> MfaRegenerateRecoveryCodesAsync([FromBody] MfaCodeApiRequest mfaCodeApiRequest);

    /// <summary>
    /// Setups the mfa and recovery codes
    /// </summary>
    /// <param name="mfaSetupApiRequest">The email information for the setup</param>
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
}
