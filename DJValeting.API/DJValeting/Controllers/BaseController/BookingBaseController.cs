using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using DJValeting.ApiObjects;

namespace DJValeting.Controllers.BaseController
{
    public abstract class BookingBaseController : ControllerBase
    {
        [HttpPost]
        [Route("/DJValeting/bookings")]
        [Consumes("application/json")]
        [ProducesResponseType(statusCode: 201, type: typeof(Guid))]
        [ProducesResponseType(statusCode: 400)]
        [ProducesResponseType(statusCode: 409)]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> CreateAsync([FromBody] BookingApi bookingApi);

        [HttpPut()]
        [Route("/DJValeting/bookings/{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(statusCode: 400)]
        [ProducesResponseType(statusCode: 404)]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> Update([FromRoute(Name = "id")][Required][MinLength(1)] string id, [FromBody] BookingApi bookingApi);

        [HttpDelete()]
        [Route("/DJValeting/bookings/{id}")]
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(statusCode: 400)]
        [ProducesResponseType(statusCode: 404)]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> Delete([FromRoute(Name = "id")][Required][MinLength(1)] string id);

        [HttpGet]
        [Route("/DJValeting/bookings")]
        [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<BookingApi>))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> ListAllAsync();

        [HttpGet]
        [Route("/DJValeting/bookings/{id}")]
        [ProducesResponseType(statusCode: 200, type: typeof(BookingApi))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> FindByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);
    }
}
