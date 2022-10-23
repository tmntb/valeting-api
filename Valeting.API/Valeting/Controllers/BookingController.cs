using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

using Valeting.Helpers;
using Valeting.ApiObjects;
using Valeting.Business.Booking;
using Valeting.Helpers.Interfaces;
using Valeting.ApiObjects.Booking;
using Valeting.Services.Interfaces;
using Valeting.Business.Flexibility;
using Valeting.Business.VehicleSize;
using Valeting.ApiObjects.Flexibility;
using Valeting.ApiObjects.VehicleSize;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers
{
    public class BookingController : BookingBaseController
    {
        private readonly IBookingService _bookingService;
        private IRedisCache _cache;

        public BookingController(IRedisCache cache, IBookingService bookingService)
        {
            this._cache = cache;
            this._bookingService = bookingService;
        }

        public override async Task<IActionResult> CreateAsync([FromBody] CreateBookingApiRequest createBookingApiRequest)
        {
            try
            {
                var bookingDTO = new BookingDTO()
                {
                    Name = createBookingApiRequest.Name,
                    BookingDate = createBookingApiRequest.BookingDate,
                    Flexibility = new FlexibilityDTO() { Id = createBookingApiRequest.Flexibility.Id },
                    VehicleSize = new VehicleSizeDTO() { Id = createBookingApiRequest.VehicleSize.Id },
                    ContactNumber = createBookingApiRequest.ContactNumber,
                    Email = createBookingApiRequest.Email
                };

                var booking = await _bookingService.CreateAsync(bookingDTO);

                //Limpar redis cache caso exista lista de bookings em cache, validar de como ter todas as keys para "List_"
                var recordKey = "*ListBooking_*";
                await _cache.RemoveRecordAsync(recordKey);

                return StatusCode((int)HttpStatusCode.Created, booking.Id);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public override async Task<IActionResult> Update([FromRoute(Name = "id"), MinLength(1), Required] string id, [FromBody] UpdateBookingApiRequest updateBookingApiRequest)
        {
            try
            {
                var bookingDTO = new BookingDTO()
                {
                    Id = Guid.Parse(id),
                    Name = updateBookingApiRequest.Name,
                    BookingDate = updateBookingApiRequest.BookingDate,
                    Flexibility = new FlexibilityDTO() { Id = updateBookingApiRequest.Flexibility.Id },
                    VehicleSize = new VehicleSizeDTO() { Id = updateBookingApiRequest.VehicleSize.Id },
                    ContactNumber = updateBookingApiRequest.ContactNumber,
                    Email = updateBookingApiRequest.Email,
                    Approved = updateBookingApiRequest.Approved
                };

                await _bookingService.UpdateAsync(bookingDTO);

                //Limpar redis cache caso exista lista e para individual
                var recordKeyList = "*ListBooking_*";
                await _cache.RemoveRecordAsync(recordKeyList);
                var recordKeyInd = string.Format("*Booking_{0}*", id);
                await _cache.RemoveRecordAsync(recordKeyInd);

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
                //Limpar redis cache caso exista lista e para individual
                var recordKeyList = "*ListBooking_*";
                await _cache.RemoveRecordAsync(recordKeyList);
                var recordKeyInd = string.Format("*Booking_{0}*", id);
                await _cache.RemoveRecordAsync(recordKeyInd);

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
                var bookingApiResponse = new BookingApiResponse()
                {
                    Booking = new BookingApi()
                };

                var recordKey = string.Format("Booking_{0}", id);

                var bookingDTO = await _cache.GetRecordAsync<BookingDTO>(recordKey);
                if(bookingDTO == null)
                {
                    bookingDTO = await _bookingService.FindByIDAsync(Guid.Parse(id));
                    await _cache.SetRecordAsync<BookingDTO>(recordKey, bookingDTO, TimeSpan.FromDays(1));
                }

                var bookingApi = new BookingApi()
                {
                    Id = bookingDTO.Id,
                    Name = bookingDTO.Name,
                    BookingDate = bookingDTO.BookingDate,
                    Flexibility = new FlexibilityApi() { Id = bookingDTO.Flexibility.Id, Description = bookingDTO.Flexibility.Description },
                    VehicleSize = new VehicleSizeApi() { Id = bookingDTO.VehicleSize.Id, Description = bookingDTO.VehicleSize.Description },
                    ContactNumber = bookingDTO.ContactNumber,
                    Email = bookingDTO.Email,
                    Approved = bookingDTO.Approved
                };

                bookingApiResponse.Booking = bookingApi;

                return StatusCode((int)HttpStatusCode.OK, bookingApiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public override async Task<IActionResult> ListAllAsync([FromQuery] BookingApiParameters bookingApiParameters)
        {
            try
            {
                var bookingApiPaginatedResponse = new BookingApiPaginatedResponse()
                {
                    Bookings = new List<BookingApi>(),
                    CurrentPage = bookingApiParameters.PageNumber,
                    Links = new PaginationLinksApi()
                };

                var bookingFilterDTO = new BookingFilterDTO()
                {
                    PageNumber = bookingApiParameters.PageNumber,
                    PageSize = bookingApiParameters.PageSize
                };

                var recordKey = string.Format("ListBooking_{0}_{1}", bookingApiParameters.PageNumber, bookingApiParameters.PageSize);

                var bookingListDTO = await _cache.GetRecordAsync<BookingListDTO>(recordKey);
                if (bookingListDTO == null)
                {
                    bookingListDTO = await _bookingService.ListAllAsync(bookingFilterDTO);
                    await _cache.SetRecordAsync<BookingListDTO>(recordKey, bookingListDTO, TimeSpan.FromMinutes(5));
                }

                bookingApiPaginatedResponse.TotalItems = bookingListDTO.TotalItems;
                bookingApiPaginatedResponse.TotalPages = bookingListDTO.TotalPages;

                bookingApiPaginatedResponse.Bookings.AddRange(
                    bookingListDTO.Bookings.Select(item => new BookingApi()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            BookingDate = item.BookingDate,
                            Flexibility = new FlexibilityApi() { Id = item.Flexibility.Id, Description = item.Flexibility.Description },
                            VehicleSize = new VehicleSizeApi() { Id = item.VehicleSize.Id, Description = item.VehicleSize.Description },
                            ContactNumber = item.ContactNumber,
                            Email = item.Email,
                            Approved = item.Approved
                        }
                    ).ToList()
                );

                return StatusCode((int)HttpStatusCode.OK, bookingApiPaginatedResponse);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
