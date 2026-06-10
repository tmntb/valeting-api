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
    [Route("/users/login")]
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
    [Route("/users/refresh-token")]
    [Consumes("application/json")]
    [ProducesResponseType(statusCode: 200, type: typeof(RefreshTokenApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 401, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenApiRequest refreshTokenApiRequest);

    

    /// <summary>
    /// Resets the password of the currently authenticated user.
    /// </summary>
    /// <param name="resetApiRequest">The request containing the current and new password.</param>
    /// <response code="204">Indicates that the password was successfully updated.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="404">Returned when the user does not exist.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPatch]
    [Route("/users/reset")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> ResetAsync([FromBody] ResetApiRequest resetApiRequest);

    /// <summary>
    /// Updates the active status of a user. Only accessible by users with the ADMIN role.
    /// </summary>
    /// <param name="updateUserApiActiveRequest">The request containing the user ID and the new active status.</param>
    /// <response code="204">Indicates that the user's active status was successfully updated.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="404">Returned when the user to be updated does not exist.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPatch]
    [Authorize(Roles = "ADMIN")]
    [Route("/users/admin-settings")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> UpdateAdminSettingsAsync([FromBody] UpdateAdminSettingsApiRequest updateAdminSettingsApiRequest);

    /// <summary>
    /// Updates the email address of the currently authenticated user.
    /// </summary>
    /// <param name="updateEmailApiRequest">The request containing the new email address.</param>
    /// <response code="204">Indicates that the email was successfully updated.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="404">Returned when the user does not exist.</response>
    /// <response code="409">Returned when the new email address is already in use by another user.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPatch]
    [Authorize]
    [Route("/users/email")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 409, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> UpdateEmailAsync([FromBody] UpdateEmailApiRequest updateEmailApiRequest);

    /// <summary>
    /// Updates the password of the currently authenticated user.
    /// </summary>
    /// <param name="updatePasswordApiRequest">The request containing the current and new password.</param>
    /// <response code="204">Indicates that the password was successfully updated.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="404">Returned when the user does not exist.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPatch]
    [Authorize]
    [Route("/users/password")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> UpdatePasswordAsync([FromBody] UpdatePasswordApiRequest updatePasswordApiRequest);

    /// <summary>
    /// Updates the profile details of the currently authenticated user.
    /// </summary>
    /// <param name="updateUserApiRequest">The updated user details.</param>
    /// <response code="204">Indicates that the user details were successfully updated.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="404">Returned when the user does not exist.</response>
    /// <response code="409">Returned when there is a conflict with existing data (e.g., email already in use).</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPatch]
    [Authorize]
    [Route("/users/profile")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 409, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> UpdateProfileAsync([FromBody] UpdateProfileApiRequest updateUserApiRequest);
}