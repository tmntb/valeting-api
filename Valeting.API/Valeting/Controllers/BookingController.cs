using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.Business;
using Valeting.ApiObjects;
using Valeting.Services.Interfaces;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers
{
    public class BookingController : BookingBaseController
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            this._bookingService = bookingService;
        }

        public override async Task<IActionResult> CreateAsync([FromBody] BookingApi bookingApi)
        {
            try
            {
                BookingDTO bookingDTO = new()
                {
                    Name = bookingApi.Name,
                    BookingDate = bookingApi.BookingDate,
                    Flexibility = new FlexibilityDTO() { Id = bookingApi.Flexibility.Id },
                    VehicleSize = new VehicleSizeDTO() { Id = bookingApi.VehicleSize.Id },
                    ContactNumber = bookingApi.ContactNumber,
                    Email = bookingApi.Email
                };

                BookingDTO booking = await _bookingService.CreateAsync(bookingDTO);

                return StatusCode((int)HttpStatusCode.Created, booking.Id);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public override async Task<IActionResult> Update([FromRoute(Name = "id"), MinLength(1), Required] string id, [FromBody] BookingApi bookingApi)
        {
            try
            {
                BookingDTO bookingDTO = new()
                {
                    Id = Guid.Parse(id),
                    Name = bookingApi.Name,
                    BookingDate = bookingApi.BookingDate,
                    Flexibility = new FlexibilityDTO() { Id = bookingApi.Flexibility.Id },
                    VehicleSize = new VehicleSizeDTO() { Id = bookingApi.VehicleSize.Id },
                    ContactNumber = bookingApi.ContactNumber,
                    Email = bookingApi.Email,
                    Approved = bookingApi.Approved
                };

                await _bookingService.UpdateAsync(bookingDTO);

                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public override async Task<IActionResult> Delete([FromRoute(Name = "id"), MinLength(1), Required] string id)
        {
            try
            {
                await _bookingService.DeleteAsync(Guid.Parse(id));

                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public override async Task<IActionResult> FindByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
        {
            try
            {
                BookingDTO booking = await _bookingService.FindByIDAsync(Guid.Parse(id));

                BookingApi bookingApi = new()
                {
                    Id = booking.Id,
                    Name = booking.Name,
                    BookingDate = booking.BookingDate,
                    Flexibility = new FlexibilityApi() { Id = booking.Flexibility.Id, Description = booking.Flexibility.Description },
                    VehicleSize = new VehicleSizeApi() { Id = booking.VehicleSize.Id, Description = booking.VehicleSize.Description },
                    ContactNumber = booking.ContactNumber,
                    Email = booking.Email,
                    Approved = booking.Approved
                };

                return StatusCode((int)HttpStatusCode.OK, bookingApi);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public override async Task<IActionResult> ListAllAsync()
        {
            try
            {
                List<BookingApi> bookingApis = new();

                IEnumerable<BookingDTO> bookings = await _bookingService.ListAllAsync();

                bookingApis.AddRange(
                    bookings.Select(item => new BookingApi()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        BookingDate = item.BookingDate,
                        Flexibility = new FlexibilityApi() { Id = item.Flexibility.Id, Description = item.Flexibility.Description },
                        VehicleSize = new VehicleSizeApi() { Id = item.VehicleSize.Id, Description = item.VehicleSize.Description },
                        ContactNumber = item.ContactNumber,
                        Email = item.Email,
                        Approved = item.Approved
                    })
                );

                return StatusCode((int)HttpStatusCode.OK, bookingApis);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
