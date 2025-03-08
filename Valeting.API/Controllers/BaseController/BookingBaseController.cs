using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Valeting.API.Models.Booking;

namespace Valeting.API.Controllers.BaseController;

[Produces("application/json")]
public abstract class BookingBaseController : ControllerBase
{
    [HttpPost]
    [Authorize]
    [Route("/bookings")]
    [Consumes("application/json")]
    [ProducesResponseType(statusCode: 201, type: typeof(CreateBookingApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(BookingApiError))]
    [ProducesResponseType(statusCode: 500, type: typeof(BookingApiError))]
    public abstract Task<IActionResult> CreateAsync([FromBody] CreateBookingApiRequest createBookingApiRequest);

    [HttpPut]
    [Authorize]
    [Route("/bookings/{id}")]
    [Consumes("application/json")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(BookingApiError))]
    [ProducesResponseType(statusCode: 404, type: typeof(BookingApiError))]
    [ProducesResponseType(statusCode: 500, type: typeof(BookingApiError))]
    public abstract Task<IActionResult> UpdateAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id, [FromBody] UpdateBookingApiRequest updateBookingApiRequest);

    [HttpDelete]
    [Authorize]
    [Route("/bookings/{id}")]
    [ProducesResponseType(statusCode: 204)]
    [ProducesResponseType(statusCode: 400, type: typeof(BookingApiError))]
    [ProducesResponseType(statusCode: 404, type: typeof(BookingApiError))]
    [ProducesResponseType(statusCode: 500, type: typeof(BookingApiError))]
    public abstract Task<IActionResult> DeleteAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);

    [HttpGet]
    [Authorize]
    [Route("/bookings/{id}")]
    [ProducesResponseType(statusCode: 200, type: typeof(BookingApiResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(BookingApiError))]
    [ProducesResponseType(statusCode: 404, type: typeof(BookingApiError))]
    [ProducesResponseType(statusCode: 500, type: typeof(BookingApiError))]
    public abstract Task<IActionResult> GetByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);

    [HttpGet]
    [Authorize]
    [Route("/bookings")]
    [ProducesResponseType(statusCode: 200, type: typeof(BookingApiPaginatedResponse))]
    [ProducesResponseType(statusCode: 400, type: typeof(BookingApiError))]
    [ProducesResponseType(statusCode: 500, type: typeof(BookingApiError))]
    public abstract Task<IActionResult> GetAsync([FromQuery] BookingApiParameters bookingApiParameters);
}