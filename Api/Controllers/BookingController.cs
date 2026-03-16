using Api.Controllers.BaseController;
using Api.Models.Booking;
using Api.Models.Booking.Payload;
using Api.Models.Core;
using Common.Enums;
using Common.Messages;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Models.Booking;
using Service.Models.Booking.Payload;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Api.Controllers;

public class BookingController(IBookingService bookingService, ILinkService urlService) : BookingBaseController
{
    /// <inheritdoc />
    public override async Task<IActionResult> CreateAsync([FromBody] CreateBookingApiRequest createBookingApiRequest)
    {
        ArgumentNullException.ThrowIfNull(createBookingApiRequest, Messages.InvalidRequestBody);

        var bookingDto = new BookingDto
        {
            Customer = new()
            {
                Id = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            },
            Flexibility = new()
            {
                Id = createBookingApiRequest.FlexibilityId
            },
            VehicleSize = new()
            {
                Id = createBookingApiRequest.VehicleSizeId
            },
            ScheduledAt = createBookingApiRequest.ScheduledAt,
            Notes = createBookingApiRequest.Notes
        };

        var bookingId = await bookingService.CreateAsync(bookingDto);

        var createBookingApiResponse = new CreateBookingApiResponse
        {
            Id = bookingId
        };
        return Created(urlService.GenerateSelf(new() { Request = Request, Path = "bookings", Id = createBookingApiResponse.Id }), createBookingApiResponse);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> UpdateAsync([FromRoute(Name = "id"), MinLength(1), Required] string id, [FromBody] UpdateBookingApiRequest updateBookingApiRequest)
    {
        ArgumentNullException.ThrowIfNull(id, Messages.InvalidRequestId);
        ArgumentNullException.ThrowIfNull(updateBookingApiRequest, Messages.InvalidRequestBody);

        var bookingDto = new BookingDto
        {
            Id = Guid.Parse(id),
            Flexibility = new()
            {
                Id = updateBookingApiRequest.FlexibilityId
            },
            VehicleSize = new()
            {
                Id = updateBookingApiRequest.VehicleSizeId
            },
            ScheduledAt = updateBookingApiRequest.ScheduledAt,
            Notes = updateBookingApiRequest.Notes
        };

        await bookingService.UpdateAsync(bookingDto);
        return NoContent();
    }

    /// <inheritdoc />
    public override async Task<IActionResult> UpdateStatusAsync([FromRoute(Name = "id"), MinLength(1), Required] string id, [FromBody] UpdateBookingApiStatusRequest updateBookingApiStatusRequest)
    {
        ArgumentNullException.ThrowIfNull(id, Messages.InvalidRequestId);

        await bookingService.UpdateStatusAsync(new()
        {
            Id = Guid.Parse(id),
            Status = updateBookingApiStatusRequest.Status,
            UserDto = new()
            {
                Id = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                Role = new()
                {
                    Code = Enum.Parse<RoleEnum>(User.FindFirst(ClaimTypes.Role)?.Value)
                }
            }
        });
        return NoContent();
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        ArgumentNullException.ThrowIfNull(id, Messages.InvalidRequestId);

        var bookingDto = await bookingService.GetByIdAsync(Guid.Parse(id));

        var bookingApi = BookingApi.MapToBookingApi(bookingDto);

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
        return Ok(bookingApiResponse);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetCustomerFilteredAsync([FromQuery] BookingApiParameters bookingApiParameters)
    {
        ArgumentNullException.ThrowIfNull(bookingApiParameters, Messages.InvalidRequestQueryParameters);

        var bookingFilterDto = new BookingFilterDto
        {
            CustomerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
            PageNumber = bookingApiParameters.PageNumber,
            PageSize = bookingApiParameters.PageSize
        };

        var paginatedBookingDtoResponse = await bookingService.GetCustomerFilteredAsync(bookingFilterDto);
        return CreatePaginatedBookingResponseAsync(paginatedBookingDtoResponse, bookingApiParameters, bookingFilterDto, false);
    }

    /// <inheritdoc />
    public override async Task<IActionResult> GetFilteredAsync([FromQuery] BookingApiParameters bookingApiParameters)
    {
        ArgumentNullException.ThrowIfNull(bookingApiParameters, Messages.InvalidRequestQueryParameters);

        var bookingFilterDto = new BookingFilterDto
        {
            Status = bookingApiParameters.Status,
            PageNumber = bookingApiParameters.PageNumber,
            PageSize = bookingApiParameters.PageSize
        };

        var paginatedBookingDtoResponse = await bookingService.GetFilteredAsync(bookingFilterDto);
        return CreatePaginatedBookingResponseAsync(paginatedBookingDtoResponse, bookingApiParameters, bookingFilterDto);
    }

    /// <summary>
    /// Creates a paginated response for a list of bookings based on the provided filter and pagination criteria.
    /// This method maps the booking DTOs to booking API models, generates HATEOAS links for pagination, and constructs a paginated response object to return to the client.
    /// </summary>
    /// <param name="paginatedBookingDtoResponse">The paginated response containing the list of booking DTOs and pagination metadata.</param>
    /// <param name="bookingApiParameters">The API parameters containing pagination and filtering criteria.</param>
    /// <param name="bookingFilterDto">The filter DTO used to generate pagination links.</param>
    /// <param name="includeCustomer">A boolean flag indicating whether to include customer information in the booking API models.</param>
    /// <returns>An IActionResult containing the paginated booking API response.</returns>
    private IActionResult CreatePaginatedBookingResponseAsync(BookingPaginatedDtoResponse paginatedBookingDtoResponse, BookingApiParameters bookingApiParameters, BookingFilterDto bookingFilterDto, bool includeCustomer = true)
    {
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

        var bookingApis = paginatedBookingDtoResponse.Bookings.Select(x => BookingApi.MapToBookingApi(x, includeCustomer)).ToList();

        bookingApis.ForEach(b =>
        {
            b.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new() { Request = Request, Id = b.Id })
                }
            };
        });

        bookingApiPaginatedResponse.Bookings = bookingApis;
        return Ok(bookingApiPaginatedResponse);
    }
}
