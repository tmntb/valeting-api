using Api.Controllers.BaseController;
using Api.Models.Booking;
using Api.Models.Core;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.Booking;
using Service.Models.Booking.Payload;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Api.Controllers;

public class BookingController(IBookingService bookingService, ILinkService urlService) : BookingBaseController
{
    public override async Task<IActionResult> CreateAsync([FromBody] CreateBookingApiRequest createBookingApiRequest)
    {
        ArgumentNullException.ThrowIfNull(createBookingApiRequest, Messages.InvalidRequestBody);

        var bookingDto = new BookingDto
        {
            Name = createBookingApiRequest.Name,
            BookingDate = createBookingApiRequest.BookingDate,
            Flexibility = new()
            {
                Id = createBookingApiRequest.Flexibility.Id
            },
            VehicleSize = new()
            {
                Id = createBookingApiRequest.VehicleSize.Id
            },
            ContactNumber = createBookingApiRequest.ContactNumber,
            Email = createBookingApiRequest.Email
        };

        var bookingId = await bookingService.CreateAsync(bookingDto);

        var createBookingApiResponse = new CreateBookingApiResponse
        {
            Id = bookingId
        };
        return StatusCode((int)HttpStatusCode.Created, createBookingApiResponse);
    }

    public override async Task<IActionResult> UpdateAsync([FromRoute(Name = "id"), MinLength(1), Required] string id, [FromBody] UpdateBookingApiRequest updateBookingApiRequest)
    {
        ArgumentNullException.ThrowIfNull(id, Messages.InvalidRequestId);
        ArgumentNullException.ThrowIfNull(updateBookingApiRequest, Messages.InvalidRequestBody);

        var bookingDto = new BookingDto
        {
            Id = Guid.Parse(id)
        };

        await bookingService.UpdateAsync(bookingDto);
        return StatusCode((int)HttpStatusCode.NoContent);
    }

    public override async Task<IActionResult> DeleteAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        ArgumentNullException.ThrowIfNull(id, Messages.InvalidRequestId);

        await bookingService.DeleteAsync(Guid.Parse(id));
        return StatusCode((int)HttpStatusCode.NoContent);
    }

    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        ArgumentNullException.ThrowIfNull(id, Messages.InvalidRequestId);

        var bookingDto = await bookingService.GetByIdAsync(Guid.Parse(id));

        var bookingApi = new BookingApi
        {
            Id = bookingDto.Id,
            Name = bookingDto.Name,
            BookingDate = bookingDto.BookingDate,
            Flexibility = new()
            {
                Id = bookingDto.Flexibility.Id,
                Description = bookingDto.Flexibility.Description
            },
            VehicleSize = new()
            {
                Id = bookingDto.VehicleSize.Id,
                Description = bookingDto.VehicleSize.Description
            },
            ContactNumber = bookingDto.ContactNumber.Value,
            Approved = bookingDto.Approved
        };

        bookingApi.Flexibility.Link = new()
        {
            Self = new()
            {
                Href = urlService.GenerateSelf(new() { Request = Request, Path = "flexibilities", Id = bookingApi.Flexibility.Id })
            }
        };
        bookingApi.VehicleSize.Link = new()
        {
            Self = new()
            {
                Href = urlService.GenerateSelf(new() { Request = Request, Path = "vehicleSizes", Id = bookingApi.VehicleSize.Id })
            }
        };
        bookingApi.Link = new()
        {
            Self = new()
            {
                Href = urlService.GenerateSelf(new() { Request = Request, Path = "bookings", Id = bookingApi.Id })
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
        ArgumentNullException.ThrowIfNull(bookingApiParameters, Messages.InvalidRequestQueryParameters);

        var bookingFilterDto = new BookingFilterDto
        {
            PageNumber = bookingApiParameters.PageNumber,
            PageSize = bookingApiParameters.PageSize
        };

        var paginatedBookingDtoResponse = await bookingService.GetFilteredAsync(bookingFilterDto);

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
                TotalPages = paginatedBookingDtoResponse.TotalPages,
                Filter = bookingFilterDto
            }
        );

        var links = new PaginationLinksApi
        {
            Self = new() { Href = paginatedLinks.Self },
            Next = new() { Href = paginatedLinks.Next },
            Prev = new() { Href = paginatedLinks.Prev }
        };
        bookingApiPaginatedResponse.Links = links;

        var bookingApis = paginatedBookingDtoResponse.Bookings.Select(x =>
            new BookingApi
            {
                Id = x.Id,
                Name = x.Name,
                BookingDate = x.BookingDate,
                Flexibility = new()
                {
                    Id = x.Flexibility.Id,
                    Description = x.Flexibility.Description
                },
                VehicleSize = new()
                {
                    Id = x.VehicleSize.Id,
                    Description = x.VehicleSize.Description
                },
                ContactNumber = x.ContactNumber.Value,
                Approved = x.Approved
            }
        ).ToList();

        bookingApis.ForEach(b =>
        {
            b.Flexibility.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new() { Request = Request, Path = "flexibilities", Id = b.Flexibility.Id })
                }
            };
            b.VehicleSize.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new() { Request = Request, Path = "vehicleSizes", Id = b.VehicleSize.Id })
                }
            };
            b.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new() { Request = Request, Id = b.Id })
                }
            };
        });

        bookingApiPaginatedResponse.Bookings = bookingApis;
        return StatusCode((int)HttpStatusCode.OK, bookingApiPaginatedResponse);
    }
}