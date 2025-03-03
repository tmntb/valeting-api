using AutoMapper;
using System.Net;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Valeting.Models.Core;
using Valeting.Models.Booking;
using Valeting.Core.Interfaces;
using Valeting.Common.Models.Link;
using Valeting.Common.Models.Booking;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers;

public class BookingController(IBookingService bookingService, IUrlService urlService, IMapper mapper) : BookingBaseController
{
    public override async Task<IActionResult> CreateAsync([FromBody] CreateBookingApiRequest createBookingApiRequest)
    {
        try
        {
            if (createBookingApiRequest == null)
            {
                var bookingApiError = new BookingApiError
                {
                    Detail = "Invalid request body"
                };
                return StatusCode((int)HttpStatusCode.BadRequest, bookingApiError);
            }

            var createBookingDtoRequest = mapper.Map<CreateBookingDtoRequest>(createBookingApiRequest);
            var createBookingDtoResponse = await bookingService.CreateAsync(createBookingDtoRequest);
            if (createBookingDtoResponse.HasError)
            {
                var bookingApiError = new BookingApiError
                {
                    Detail = createBookingDtoResponse.Error.Message
                };
                return StatusCode(createBookingDtoResponse.Error.ErrorCode, bookingApiError);
            }

            var createBookingApiResponse = mapper.Map<CreateBookingApiResponse>(createBookingDtoResponse);
            return StatusCode((int)HttpStatusCode.Created, createBookingApiResponse);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError
            {
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }

    public override async Task<IActionResult> UpdateAsync([FromRoute(Name = "id"), MinLength(1), Required] string id, [FromBody] UpdateBookingApiRequest updateBookingApiRequest)
    {
        try
        {
            if (updateBookingApiRequest == null)
            {
                var bookingApiError = new BookingApiError
                {
                    Detail = "Invalid request body"
                };
                return StatusCode((int)HttpStatusCode.BadRequest, bookingApiError);
            }

            var updateBookingDtoRequest = mapper.Map<UpdateBookingDtoRequest>(updateBookingApiRequest);
            updateBookingDtoRequest.Id = Guid.Parse(id);

            var updateBookingDtoResponse = await bookingService.UpdateAsync(updateBookingDtoRequest);
            if (updateBookingDtoResponse.HasError)
            {
                var bookingApiError = new BookingApiError
                {
                    Detail = updateBookingDtoResponse.Error.Message
                };
                return StatusCode(updateBookingDtoResponse.Error.ErrorCode, bookingApiError);
            }

            return StatusCode((int)HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError
            {
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }

    public override async Task<IActionResult> DeleteAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        try
        {
            var deleteBookingDtoRequest = new DeleteBookingDtoRequest
            {
                Id = Guid.Parse(id)
            };

            var deleteBookingDtoResponse = await bookingService.DeleteAsync(deleteBookingDtoRequest);
            if (deleteBookingDtoResponse.HasError)
            {
                var bookingApiError = new BookingApiError()
                {
                    Detail = deleteBookingDtoResponse.Error.Message
                };
                return StatusCode(deleteBookingDtoResponse.Error.ErrorCode, bookingApiError);
            }

            return StatusCode((int)HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError
            {
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }

    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        try
        {
            var getBookingDtoRequest = new GetBookingDtoRequest
            {
                Id = Guid.Parse(id)
            };

            var getBookingDtoResponse = await bookingService.GetByIdAsync(getBookingDtoRequest);
            if (getBookingDtoResponse.HasError)
            {
                var bookingApiError = new BookingApiError()
                {
                    Detail = getBookingDtoResponse.Error.Message
                };
                return StatusCode(getBookingDtoResponse.Error.ErrorCode, bookingApiError);
            }

            var bookingApi = mapper.Map<BookingApi>(getBookingDtoResponse);
            bookingApi.Flexibility.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new GenerateSelfUrlDtoRequest() { BaseUrl = Request.Host.Value, Path = "/flexibilities", Id = bookingApi.Flexibility.Id }).Self
                }
            };
            bookingApi.VehicleSize.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new GenerateSelfUrlDtoRequest() { BaseUrl = Request.Host.Value, Path = "/vehicleSizes", Id = bookingApi.VehicleSize.Id }).Self
                }
            };

            var bookingApiResponse = new BookingApiResponse
            {
                Booking = bookingApi
            };
            return StatusCode((int)HttpStatusCode.OK, bookingApiResponse);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError
            {
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }

    public override async Task<IActionResult> GetAsync([FromQuery] BookingApiParameters bookingApiParameters)
    {
        try
        {
            var paginatedBookingDtoRequest = mapper.Map<PaginatedBookingDtoRequest>(bookingApiParameters);


            var paginatedBookingDtoResponse = await bookingService.GetAsync(paginatedBookingDtoRequest);
            if (paginatedBookingDtoResponse.HasError)
            {
                var bookingApiError = new BookingApiError
                {
                    Detail = paginatedBookingDtoResponse.Error.Message
                };
                return StatusCode(paginatedBookingDtoResponse.Error.ErrorCode, bookingApiError);
            }

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
                new GeneratePaginatedLinksDtoRequest
                {
                    BaseUrl = Request.Host.Value,
                    Path = Request.Path.HasValue ? Request.Path.Value : string.Empty,
                    QueryString = Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty,
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
                        Href = urlService.GenerateSelf(new GenerateSelfUrlDtoRequest { BaseUrl = Request.Host.Value, Path = "/flexibilities", Id = b.Flexibility.Id }).Self
                    }
                };
                b.VehicleSize.Link = new()
                {
                    Self = new()
                    {
                        Href = urlService.GenerateSelf(new GenerateSelfUrlDtoRequest { BaseUrl = Request.Host.Value, Path = "/vehicleSizes", Id = b.VehicleSize.Id }).Self
                    }
                };
                b.Link = new()
                {
                    Self = new()
                    {
                        Href = urlService.GenerateSelf(new GenerateSelfUrlDtoRequest { BaseUrl = Request.Host.Value, Path = Request.Path.Value, Id = b.Id }).Self
                    }
                };
            });
            bookingApiPaginatedResponse.Bookings = bookingApis;

            return StatusCode((int)HttpStatusCode.OK, bookingApiPaginatedResponse);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError
            {
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }
}