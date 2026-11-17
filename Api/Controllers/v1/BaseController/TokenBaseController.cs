using Api.Models.Core;
using Api.Models.Token.Payload;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1.BaseController;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class TokenBaseController : ControllerBase
{
    /// <summary>
    /// Generates a new Jwt access token for the user.
    /// </summary>
    /// <param name="generateJwtTokenApiRequest">The request containing the necessary user info.</param>
    /// <response code="200">Returns a newly generated Jwt Token.</response>
    /// <response code="400">Returned when the request body is invalid or missing required fields.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Route("generate")]
    [Consumes("application/json")]
    [ProducesResponseType(statusCode: 200, type: typeof(JwtTokenApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> GenerateJwtTokenAsync([FromBody] GenerateJwtTokenApiRequest generateJwtTokenApiRequest);
    
    /// <summary>
    /// Refreshes the user JWT access token.
    /// </summary>
    /// <param name="refreshJwtTokenApiRequest">The request containing the current JWT token.</param>
    /// <response code="200">Returns a newly generated JWT token.</response>
    /// <response code="400">Returned when the request body is invalid or missing required fields.</response>
    /// <response code="401">Returned when the token is invalid or expired beyond the refresh window.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Route("refresh")]
    [Consumes("application/json")]
    [ProducesResponseType(statusCode: 200, type: typeof(JwtTokenApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 401, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> RefreshJwtTokenAsync([FromBody] RefreshJwtTokenApiRequest refreshJwtTokenApiRequest);
}