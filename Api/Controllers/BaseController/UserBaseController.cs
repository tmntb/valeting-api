using Api.Models.Core;
using Api.Models.User.Payload;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseController;

[Produces("application/json")]
public abstract class UserBaseController : ControllerBase
{
    /// <summary>
    /// Authenticates a user and generates a JWT access token.
    /// </summary>
    /// <remarks>
    /// This endpoint validates the provided user credentials and, if successful,  
    /// returns a JSON Web Token (JWT) that can be used to authorize subsequent API requests.  
    /// If the username or password is invalid, a <c>401 Unauthorized</c> is returned.  
    /// Validation errors or missing required fields result in a <c>400 Bad Request</c>.  
    /// Unexpected server errors result in a <c>500 Internal Server Error</c>.
    /// </remarks>
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
    public abstract Task<IActionResult> Login([FromBody] LoginApiRequest loginApiRequest);

    /// <summary>
    /// Refreshes the user JWT access token.
    /// </summary>
    /// <remarks>
    /// This endpoint validates the provided JWT token and, if valid, issues a new one.  
    /// If the token is expired beyond the allowed refresh window or invalid, a <c>401 Unauthorized</c> is returned.  
    /// Missing or invalid fields result in a <c>400 Bad Request</c>.  
    /// Unexpected server errors result in a <c>500 Internal Server Error</c>.
    /// </remarks>
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
    /// <remarks>
    /// This endpoint creates a new user with the provided credentials.  
    /// The username must be unique; if it already exists, a <c>409 Conflict</c> is returned.  
    /// Validation errors or missing required fields result in a <c>400 Bad Request</c>.  
    /// Unexpected server errors result in a <c>500 Internal Server Error</c>.
    /// </remarks>
    /// <param name="registerApiRequest">The registration information, including username and password.</param>
    /// <response code="200">Indicates that the user was successfully registered.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="409">Returned when a user with the same username already exists.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Route("/user/register")]
    [ProducesResponseType(statusCode: 200)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 409, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> Register([FromBody] RegisterApiRequest registerApiRequest);
}