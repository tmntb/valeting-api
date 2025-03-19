using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Valeting.API.Controllers.BaseController;
using Valeting.API.Models.Booking;
using Valeting.API.Models.Core;
using Valeting.Common.Models.Booking;
using Valeting.Core.Interfaces;

namespace Valeting.API.Controllers;

public class BookingController(IBookingService bookingService, IUrlService urlService, IMapper mapper) : BookingBaseController
{
    public override async Task<IActionResult> CreateAsync([FromBody] CreateBookingApiRequest createBookingApiRequest)
    {
        ArgumentNullException.ThrowIfNull(createBookingApiRequest, "Invalid request body");
        
        var createBookingDtoRequest = mapper.Map<CreateBookingDtoRequest>(createBookingApiRequest);
        var createBookingDtoResponse = await bookingService.CreateAsync(createBookingDtoRequest);
        
        var createBookingApiResponse = mapper.Map<CreateBookingApiResponse>(createBookingDtoResponse);
        return StatusCode((int)HttpStatusCode.Created, createBookingApiResponse);
    }

    public override async Task<IActionResult> UpdateAsync([FromRoute(Name = "id"), MinLength(1), Required] string id, [FromBody] UpdateBookingApiRequest updateBookingApiRequest)
    {
        ArgumentNullException.ThrowIfNull(id, "Invalid request id");
        ArgumentNullException.ThrowIfNull(updateBookingApiRequest, "Invalid request body");

        var updateBookingDtoRequest = mapper.Map<UpdateBookingDtoRequest>(updateBookingApiRequest);
        updateBookingDtoRequest.Id = Guid.Parse(id);

        await bookingService.UpdateAsync(updateBookingDtoRequest);
        return StatusCode((int)HttpStatusCode.NoContent);
    }

    public override async Task<IActionResult> DeleteAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        ArgumentNullException.ThrowIfNull(id, "Invalid request id");

        var deleteBookingDtoRequest = new DeleteBookingDtoRequest
        {
            Id = Guid.Parse(id)
        };

        await bookingService.DeleteAsync(deleteBookingDtoRequest);
        return StatusCode((int)HttpStatusCode.NoContent);
    }

    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        ArgumentNullException.ThrowIfNull(id, "Invalid request id");

        var getBookingDtoRequest = new GetBookingDtoRequest
        {
            Id = Guid.Parse(id)
        };

        var getBookingDtoResponse = await bookingService.GetByIdAsync(getBookingDtoRequest);  

        var bookingApi = mapper.Map<BookingApi>(getBookingDtoResponse.Booking);
        bookingApi.Flexibility.Link = new()
        {
            Self = new()
            {
                Href = urlService.GenerateSelf(new() { Request = Request, Path = "flexibilities", Id = bookingApi.Flexibility.Id }).Self
            }
        };
        bookingApi.VehicleSize.Link = new()
        {
            Self = new()
            {
                Href = urlService.GenerateSelf(new() { Request = Request, Path = "vehicleSizes", Id = bookingApi.VehicleSize.Id }).Self
            }
        };
        bookingApi.Link = new()
        {
            Self = new()
            {
                Href = urlService.GenerateSelf(new() { Request = Request, Path = "bookings", Id = bookingApi.Id }).Self
            }
        };

        var bookingApiResponse = new BookingApiResponse
        {
            Booking = bookingApi
        };
        return StatusCode((int)HttpStatusCode.OK, bookingApiResponse);
    }

    public override async Task<IActionResult> GetFilteredAsync([FromQuery] BookingApiParameters bookingApiParameters)
    {
        ArgumentNullException.ThrowIfNull(bookingApiParameters, "Invalid request query parameters");

        var paginatedBookingDtoRequest = mapper.Map<PaginatedBookingDtoRequest>(bookingApiParameters);

        var paginatedBookingDtoResponse = await bookingService.GetFilteredAsync(paginatedBookingDtoRequest);

        var bookingApiPaginatedResponse = new BookingApiPaginatedResponse
        {
            Bookings = [],
            CurrentPage = bookingApiParameters.PageNumber,
            TotalItems = paginatedBookingDtoResponse.TotalItems,
            TotalPages = paginatedBookingDtoResponse.TotalPages,
            Links = new()
            {
                Prev = new() { Href = string.Empty },
                Next = new() { Href = string.Empty },
                Self = new() { Href = string.Empty }
            }
        };

        var paginatedLinks = urlService.GeneratePaginatedLinks
        (
            new()
            {
                Request = Request,
                PageNumber = bookingApiParameters.PageNumber,
                TotalPages = paginatedBookingDtoResponse.TotalPages,
                Filter = paginatedBookingDtoRequest.Filter
            }
        );

        var links = mapper.Map<PaginationLinksApi>(paginatedLinks);
        bookingApiPaginatedResponse.Links = links;

        var bookingApis = mapper.Map<List<BookingApi>>(paginatedBookingDtoResponse.Bookings);
        bookingApis.ForEach(b =>
        {
            b.Flexibility.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new() { Request = Request, Path = "flexibilities", Id = b.Flexibility.Id }).Self
                }
            };
            b.VehicleSize.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new() { Request = Request, Path = "vehicleSizes", Id = b.VehicleSize.Id }).Self
                }
            };
            b.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new() { Request = Request, Id = b.Id }).Self
                }
            };
        });

        bookingApiPaginatedResponse.Bookings = bookingApis;
        return StatusCode((int)HttpStatusCode.OK, bookingApiPaginatedResponse);
    }
}