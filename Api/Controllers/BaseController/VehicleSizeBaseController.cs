using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Api.Models.Core;
using Api.Models.VehicleSize.Payload;

namespace Api.Controllers.BaseController;

[Produces("application/json")]
public abstract class VehicleSizeBaseController : ControllerBase
{
    /// <summary>
    /// Retrieves a specific vehicle size by its identifier.
    /// </summary>
    /// <remarks>
    /// This endpoint fetches the details of a single vehicle size.  
    /// If the requested vehicle size does not exist, a <c>404 Not Found</c> will be returned.  
    /// Validation errors for the request ID will result in a <c>400 Bad Request</c>.  
    /// Unexpected server errors will result in a <c>500 Internal Server Error</c>.
    /// </remarks>
    /// <param name="id">The identifier of the vehicle size to retrieve.</param>
    /// <response code="200">Returns the requested vehicle size details.</response>
    /// <response code="400">Returned when the request ID is invalid or missing.</response>
    /// <response code="404">Returned when the vehicle size with the specified ID is not found.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpGet]
    [Authorize]
    [Route("/vehicleSizes/{id}")]
    [ProducesResponseType(statusCode: 200, type: typeof(VehicleSizeApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> GetByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);

    /// <summary>
    /// Retrieves a paginated list of vehicle sizes.
    /// </summary>
    /// <remarks>
    /// This endpoint fetches a list of vehicle sizes, optionally filtered by active status.  
    /// Pagination parameters can be specified via query parameters.  
    /// Validation errors will result in a <c>400 Bad Request</c>.  
    /// Unexpected server errors will result in a <c>500 Internal Server Error</c>.
    /// </remarks>
    /// <param name="vehicleSizeApiParameters">Pagination and filtering parameters.</param>
    /// <response code="200">Returns a paginated list of vehicle sizes with links for navigation.</response>
    /// <response code="400">Returned when the query parameters are invalid.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpGet]
    [Authorize]
    [Route("/vehicleSizes")]
    [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<VehicleSizeApiPaginatedResponse>))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> GetFilteredAsync([FromQuery] VehicleSizeApiParameters vehicleSizeApiParameters);
}