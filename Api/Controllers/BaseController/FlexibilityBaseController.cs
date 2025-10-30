using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Api.Models.Core;
using Api.Models.Flexibility.Payload;

namespace Api.Controllers.BaseController;

[Produces("application/json")]
public abstract class FlexibilityBaseController : ControllerBase
{
    /// <summary>
    /// Retrieves a specific flexibility by its unique identifier.
    /// </summary>
    /// <remarks>
    /// This endpoint returns detailed information about a specific flexibility identified by its unique ID.  
    /// If the flexibility does not exist, a <c>404 Not Found</c> is returned.  
    /// If the provided ID is invalid or improperly formatted, a <c>400 Bad Request</c> is returned.  
    /// Unexpected server errors result in a <c>500 Internal Server Error</c>.
    /// </remarks>
    /// <param name="id">The unique identifier of the flexibility to retrieve.</param>
    /// <response code="200">Returns the details of the requested flexibility.</response>
    /// <response code="400">Returned when the provided ID is invalid or missing.</response>
    /// <response code="404">Returned when no flexibility with the given ID exists.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpGet]
    [Authorize]
    [Route("/flexibilities/{id}")]
    [ProducesResponseType(statusCode: 200, type: typeof(FlexibilityApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> GetByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);

    /// <summary>
    /// Retrieves a paginated list of flexibilities based on the provided query parameters.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a paginated collection of flexibilities.  
    /// You can filter the results by their active status and control pagination using the provided query parameters.  
    /// Each item includes metadata and navigation links.  
    /// If the query parameters are invalid, a <c>400 Bad Request</c> is returned.  
    /// Unexpected server errors result in a <c>500 Internal Server Error</c>.
    /// </remarks>
    /// <param name="flexibilityApiParameters">The pagination and filter parameters for retrieving flexibilities.</param>
    /// <response code="200">Returns a paginated list of flexibilities along with pagination metadata and navigation links.</response>
    /// <response code="400">Returned when query parameters are invalid or missing.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpGet]
    [Authorize]
    [Route("/flexibilities")]
    [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<FlexibilityApiPaginatedResponse>))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> GetFilteredAsync([FromQuery] FlexibilityApiParameters flexibilityApiParameters);
}