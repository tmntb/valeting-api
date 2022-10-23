using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Valeting.ApiObjects.Booking;

namespace Valeting.Controllers.BaseController
{
    public abstract class BookingBaseController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        [Route("/Valeting/bookings")]
        [Consumes("application/json")]
        [ProducesResponseType(statusCode: 201, type: typeof(CreateBookingApiResponse))]
        [ProducesResponseType(statusCode: 400, type: typeof(BookingApiError))]
        [ProducesResponseType(statusCode: 409, type: typeof(BookingApiError))]
        [ProducesResponseType(statusCode: 500, type: typeof(BookingApiError))]
        public abstract Task<IActionResult> CreateAsync([FromBody] CreateBookingApiRequest createBookingApiRequest);

        [HttpPut()]
        [Authorize]
        [Route("/Valeting/bookings/{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(statusCode: 400, type: typeof(BookingApiError))]
        [ProducesResponseType(statusCode: 404, type: typeof(BookingApiError))]
        [ProducesResponseType(statusCode: 500, type: typeof(BookingApiError))]
        public abstract Task<IActionResult> Update([FromRoute(Name = "id")][Required][MinLength(1)] string id, [FromBody] UpdateBookingApiRequest updateBookingApiRequest);

        [HttpDelete()]
        [Authorize]
        [Route("/Valeting/bookings/{id}")]
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(statusCode: 400, type: typeof(BookingApiError))]
        [ProducesResponseType(statusCode: 404, type: typeof(BookingApiError))]
        [ProducesResponseType(statusCode: 500, type: typeof(BookingApiError))]
        public abstract Task<IActionResult> Delete([FromRoute(Name = "id")][Required][MinLength(1)] string id);

        [HttpGet]
        [Authorize]
        [Route("/Valeting/bookings")]
        [ProducesResponseType(statusCode: 200, type: typeof(BookingApiPaginatedResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(BookingApiError))]
        public abstract Task<IActionResult> ListAllAsync([FromQuery] BookingApiParameters bookingApiParameters);

        [HttpGet]
        [Authorize]
        [Route("/Valeting/bookings/{id}")]
        [ProducesResponseType(statusCode: 200, type: typeof(BookingApiResponse))]
        [ProducesResponseType(statusCode: 500, type: typeof(BookingApiError))]
        public abstract Task<IActionResult> FindByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);
    }
}
