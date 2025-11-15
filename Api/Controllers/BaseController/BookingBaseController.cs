using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Api.Models.Core;
using Api.Models.Booking.Payload;

namespace Api.Controllers.BaseController;

[Produces("application/json")]
public abstract class BookingBaseController : ControllerBase
{
    /// <summary>
    /// Creates a new booking request.
    /// </summary>
    /// <remarks>
    /// This endpoint creates a booking with the specified details.  
    /// Validation errors or missing required fields will result in a <c>400 Bad Request</c>.  
    /// Unexpected server errors will result in a <c>500 Internal Server Error</c>.
    /// </remarks>
    /// <param name="createBookingApiRequest">The booking information to be created.</param>
    /// <response code="201">Returns the identifier of the newly created booking.</response>
    /// <response code="400">Returned when the request body is invalid or fails validation.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPost]
    [Authorize]
    [Route("/bookings")]
    [Consumes("application/json")]
    [ProducesResponseType(statusCode: 201, type: typeof(CreateBookingApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> CreateAsync([FromBody] CreateBookingApiRequest createBookingApiRequest);

    /// <summary>
    /// Updates an existing booking.
    /// </summary>
    /// <remarks>
    /// This endpoint updates an existing booking with the provided details.  
    /// If the booking ID is invalid or the request body is malformed, a <c>400 Bad Request</c> will be returned.  
    /// Unexpected server errors will result in a <c>500 Internal Server Error</c>.
    /// </remarks>
    /// <param name="id">The unique identifier of the booking to update.</param>
    /// <param name="updateBookingApiRequest">The updated booking information.</param>
    /// <response code="204">Returned when the booking is successfully updated.</response>
    /// <response code="400">Returned when the request ID or body is invalid.</response>
    /// <response code="404">Returned when the specified booking does not exist.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPut]
    [Authorize]
    [Route("/bookings/{id}")]
    [Consumes("application/json")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> UpdateAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id, [FromBody] UpdateBookingApiRequest updateBookingApiRequest);

    /// <summary>
    /// Deletes an existing booking.
    /// </summary>
    /// <remarks>
    /// This endpoint permanently removes a booking identified by its unique ID.  
    /// If the provided ID is invalid, a <c>400 Bad Request</c> will be returned.  
    /// If no booking is found with the specified ID, a <c>404 Not Found</c> will be returned.  
    /// Unexpected server errors will result in a <c>500 Internal Server Error</c>.
    /// </remarks>
    /// <param name="id">The unique identifier of the booking to delete.</param>
    /// <response code="204">Returned when the booking is successfully deleted.</response>
    /// <response code="400">Returned when the booking ID is invalid.</response>
    /// <response code="404">Returned when the booking does not exist.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpDelete]
    [Authorize]
    [Route("/bookings/{id}")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> DeleteAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);

    /// <summary>
    /// Retrieves a specific booking by its unique identifier.
    /// </summary>
    /// <remarks>
    /// This endpoint returns detailed information about a booking, including its flexibility and vehicle size options.  
    /// If the provided ID is invalid, a <c>400 Bad Request</c> will be returned.  
    /// If no booking is found for the specified ID, a <c>404 Not Found</c> will be returned.  
    /// Unexpected server errors will result in a <c>500 Internal Server Error</c>.
    /// </remarks>
    /// <param name="id">The unique identifier of the booking to retrieve.</param>
    /// <response code="200">Returns the booking details matching the provided ID.</response>
    /// <response code="400">Returned when the booking ID is invalid or improperly formatted.</response>
    /// <response code="404">Returned when no booking exists with the specified ID.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpGet]
    [Authorize]
    [Route("/bookings/{id}")]
    [ProducesResponseType(statusCode: 200, type: typeof(BookingApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> GetByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);

    /// <summary>
    /// Retrieves a paginated list of bookings based on the provided query parameters.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a paginated collection of bookings with optional filters such as page number and page size.  
    /// Each booking includes details about its flexibility and vehicle size, along with pagination metadata and navigation links.  
    /// If query parameters are invalid or missing, a <c>400 Bad Request</c> will be returned.  
    /// Unexpected server errors will result in a <c>500 Internal Server Error</c>.
    /// </remarks>
    /// <param name="bookingApiParameters">The pagination and filter parameters for retrieving bookings.</param>
    /// <response code="200">Returns a paginated list of bookings along with pagination metadata and links.</response>
    /// <response code="400">Returned when query parameters are invalid or improperly formatted.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpGet]
    [Authorize]
    [Route("/bookings")]
    [ProducesResponseType(statusCode: 200, type: typeof(BookingApiPaginatedResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> GetFilteredAsync([FromQuery] BookingApiParameters bookingApiParameters);
}