using Api.Models.Auth.Payload;
using Api.Models.Core;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseController;

[ApiController]
[Route("api/v{version:apiVersion}")]
public abstract class AuthBaseController : ControllerBase
{
    /// <summary>
    /// Setups the mfa and recovery codes
    /// </summary>
    /// <param name="mfaSetupApiRequest">The email information for the setup</param>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="409">Returned when the user mfa is already activated.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Route("auth/mfa/setup")]
    [ProducesResponseType(statusCode: 200, type: typeof(MfaSetupApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 409, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> MfaSetupAsync([FromBody] MfaSetupApiRequest mfaSetupApiRequest);

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="registerApiRequest">The registration information, including username and password.</param>
    /// <response code="200">Indicates that the user was successfully registered.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="409">Returned when a user with the same username already exists.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Route("auth/register")]
    [ProducesResponseType(statusCode: 201)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 409, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> RegisterAsync([FromBody] RegisterApiRequest registerApiRequest);
}
