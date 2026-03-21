using Api.Models.Core;
using Api.Models.User.Payload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseController;

[Produces("application/json")]
public abstract class UserBaseController : ControllerBase
{
    /// <summary>
    /// Authenticates a user and generates a JWT access token.
    /// </summary>
    /// <param name="loginApiRequest">The login credentials (username and password) of the user.</param>
    /// <response code="200">Returns a JWT access token and related metadata.</response>
    /// <response code="400">Returned when the request body is invalid or missing required fields.</response>
    /// <response code="401">Returned when authentication fails due to invalid credentials.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Route("/user/login")]
    [ProducesResponseType(statusCode: 200, type: typeof(LoginApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> LoginAsync([FromBody] LoginApiRequest loginApiRequest);

    /// <summary>
    /// Refreshes the user JWT access token.
    /// </summary>
    /// <param name="refreshTokenApiRequest">The request containing the current JWT token.</param>
    /// <response code="200">Returns a newly generated JWT token.</response>
    /// <response code="400">Returned when the request body is invalid or missing required fields.</response>
    /// <response code="401">Returned when the token is invalid or expired beyond the refresh window.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Route("/user/refreshToken")]
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
    /// <response code="200">Indicates that the user was successfully registered.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="409">Returned when a user with the same username already exists.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Route("/user/register")]
    [ProducesResponseType(statusCode: 201)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 409, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> RegisterAsync([FromBody] RegisterApiRequest registerApiRequest);

    /// <summary>
    /// Resets the password of the currently authenticated user.
    /// </summary>
    /// <param name="resetApiRequest">The request containing the current and new password.</param>
    /// <response code="204">Indicates that the password was successfully updated.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="404">Returned when the user does not exist.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPatch]
    [Route("/user/reset")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> ResetAsync([FromBody] ResetApiRequest resetApiRequest);
}