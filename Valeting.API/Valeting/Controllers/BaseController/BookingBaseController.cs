using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Valeting.ApiObjects;

namespace Valeting.Controllers.BaseController
{
    public abstract class BookingBaseController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        [Route("/Valeting/bookings")]
        [Consumes("application/json")]
        [ProducesResponseType(statusCode: 201, type: typeof(Guid))]
        [ProducesResponseType(statusCode: 400)]
        [ProducesResponseType(statusCode: 409)]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> CreateAsync([FromBody] BookingApi bookingApi);

        [HttpPut()]
        [Authorize]
        [Route("/Valeting/bookings/{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(statusCode: 400)]
        [ProducesResponseType(statusCode: 404)]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> Update([FromRoute(Name = "id")][Required][MinLength(1)] string id, [FromBody] BookingApi bookingApi);

        [HttpDelete()]
        [Authorize]
        [Route("/Valeting/bookings/{id}")]
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(statusCode: 400)]
        [ProducesResponseType(statusCode: 404)]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> Delete([FromRoute(Name = "id")][Required][MinLength(1)] string id);

        [HttpGet]
        [Authorize]
        [Route("/Valeting/bookings")]
        [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<BookingApi>))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> ListAllAsync();

        [HttpGet]
        [Authorize]
        [Route("/Valeting/bookings/{id}")]
        [ProducesResponseType(statusCode: 200, type: typeof(BookingApi))]
        [ProducesResponseType(statusCode: 500)]
        public abstract Task<IActionResult> FindByIdAsync([FromRoute(Name = "id")][Required][MinLength(1)] string id);
    }
}
