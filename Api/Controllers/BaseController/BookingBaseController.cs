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
    /// <param name="id">The unique identifier of the booking to update.</param>
    /// <param name="updateBookingApiRequest">The updated booking information.</param>
    /// <response code="204">Returned when the booking is successfully updated.</response>
    /// <response code="400">Returned when the request ID or body is invalid.</response>
    /// <response code="404">Returned when the specified booking does not exist.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPatch]
    [Authorize]
    [Route("/bookings/{id}")]
    [Consumes("application/json")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> UpdateAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id, [FromBody] UpdateBookingApiRequest updateBookingApiRequest);

    /// <summary>
    /// Updates the status of an existing booking.
    /// </summary>
    /// <param name="id">The unique identifier of the booking to update.</param>
    /// <param name="updateBookingApiStatusRequest">The new status information for the booking.</param>
    /// <response code="204">Returned when the booking is successfully updated.</response>
    /// <response code="400">Returned when the booking ID is invalid.</response>
    /// <response code="404">Returned when the booking does not exist.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpPatch]
    [Authorize]
    [Route("/bookings/{id}/status")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 404, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> UpdateStatusAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id, [FromBody] UpdateBookingApiStatusRequest updateBookingApiStatusRequest);

    /// <summary>
    /// Retrieves a specific booking by its unique identifier.
    /// </summary>
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
    /// Retrieves a paginated list of bookings for a customer based on the provided query parameters.
    /// </summary>
    /// <param name="bookingApiParameters">The pagination and filter parameters for retrieving customer bookings.</param>
    /// <response code="200">Returns a paginated list of bookings along with pagination metadata and links.</response>
    /// <response code="400">Returned when query parameters are invalid or improperly formatted.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpGet]
    [Authorize]
    [Route("/bookings/customer")]
    [ProducesResponseType(statusCode: 200, type: typeof(BookingApiPaginatedResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> GetCustomerFilteredAsync([FromQuery] BookingApiParameters bookingApiParameters);

    /// <summary>
    /// Retrieves a paginated list of bookings based on the provided query parameters, with additional filters available to admin users.
    /// </summary>
    /// <response code="200">Returns a paginated list of bookings along with pagination metadata and links.</response>
    /// <response code="400">Returned when query parameters are invalid or improperly formatted.</response>
    /// <response code="500">Returned when an unexpected error occurs.</response>
    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    [Route("/bookings")]
    [ProducesResponseType(statusCode: 200, type: typeof(BookingApiPaginatedResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(ErrorApi))]
    [ProducesResponseType(statusCode: 500, type: typeof(ErrorApi))]
    public abstract Task<IActionResult> GetFilteredAsync([FromQuery] BookingApiParameters bookingApiParameters);
}