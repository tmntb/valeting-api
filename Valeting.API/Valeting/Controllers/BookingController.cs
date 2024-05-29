using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.Helpers.Interfaces;
using Valeting.ApiObjects.Booking;
using Valeting.Services.Interfaces;
using Valeting.Services.Objects.Link;
using Valeting.Services.Objects.Booking;
using Valeting.Controllers.BaseController;
using AutoMapper;

namespace Valeting.Controllers;

public class BookingController(IRedisCache redisCache, IBookingService bookingService, IUrlService urlService, IMapper mapper) : BookingBaseController
{
    public override async Task<IActionResult> CreateAsync([FromBody] CreateBookingApiRequest createBookingApiRequest)
    {
        try
        {
            if(createBookingApiRequest == null)
            {
                var bookingApiError = new BookingApiError() 
                { 
                    Detail = "Invalid request body"
                }; 
                return StatusCode((int)HttpStatusCode.BadRequest, bookingApiError);
            }

            var test = mapper.Map<CreateBookingSVRequest>(createBookingApiRequest);
            var createBookingSVRequest = new CreateBookingSVRequest()
            {
                Name = createBookingApiRequest.Name,
                BookingDate = createBookingApiRequest.BookingDate,
                Flexibility = createBookingApiRequest.Flexibility != null ? new() { Id = createBookingApiRequest.Flexibility.Id } : new(),
                VehicleSize = createBookingApiRequest.VehicleSize != null ? new() { Id = createBookingApiRequest.VehicleSize.Id } : new(),
                ContactNumber = createBookingApiRequest.ContactNumber,
                Email = createBookingApiRequest.Email
            };

            var createBookingSVResponse = await bookingService.CreateAsync(createBookingSVRequest);
            if(createBookingSVResponse.HasError)
            {
                var bookingApiError = new BookingApiError() 
                { 
                    Detail = createBookingSVResponse.Error.Message
                };
                return StatusCode(createBookingSVResponse.Error.ErrorCode, bookingApiError);
            }

            //Limpar redis cache caso exista lista de bookings em cache, validar de como ter todas as keys para "List_"
            var recordKey = "*ListBooking_*";
            await redisCache.RemoveRecord(recordKey);

            var createBookingApiResponse = new CreateBookingApiResponse()
            {
                Id = createBookingSVResponse.Id
            };
            return StatusCode((int)HttpStatusCode.Created, createBookingApiResponse);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = ex.StackTrace
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }

    public override async Task<IActionResult> Update([FromRoute(Name = "id"), MinLength(1), Required] string id, [FromBody] UpdateBookingApiRequest updateBookingApiRequest)
    {
        try
        {
            if(updateBookingApiRequest == null)
            {
                var bookingApiError = new BookingApiError() 
                { 
                    Detail = "Invalid request body"
                }; 
                return StatusCode((int)HttpStatusCode.BadRequest, bookingApiError);
            }

            var updateBookingSVRequest = new UpdateBookingSVRequest()
            {
                Id = Guid.Parse(id),
                Name = updateBookingApiRequest.Name,
                BookingDate = updateBookingApiRequest.BookingDate,
                Flexibility = updateBookingApiRequest.Flexibility != null ? new() { Id = updateBookingApiRequest.Flexibility.Id } : new(),
                VehicleSize = updateBookingApiRequest.VehicleSize != null ? new() { Id = updateBookingApiRequest.VehicleSize.Id } : new(),
                ContactNumber = updateBookingApiRequest.ContactNumber,
                Email = updateBookingApiRequest.Email,
                Approved = updateBookingApiRequest.Approved ?? false
            };

            var updateBookingSVResponse = await bookingService.UpdateAsync(updateBookingSVRequest);
            if(updateBookingSVResponse.HasError)
            {
                var bookingApiError = new BookingApiError() 
                { 
                    Detail = updateBookingSVResponse.Error.Message
                };
                return StatusCode(updateBookingSVResponse.Error.ErrorCode, bookingApiError);
            }

            //Limpar redis cache caso exista lista e para individual
            var recordKeyList = "*ListBooking_*";
            await redisCache.RemoveRecord(recordKeyList);
            var recordKeyInd = string.Format("*Booking_{0}*", id);
            await redisCache.RemoveRecord(recordKeyInd);

            return StatusCode((int)HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = ex.StackTrace
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }

    public override async Task<IActionResult> Delete([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        try
        {
            var deleteBookingSVRequest = new DeleteBookingSVRequest()
            {
                Id = Guid.Parse(id)
            };

            var deleteBookingSVResponse = await bookingService.DeleteAsync(deleteBookingSVRequest);
            if(deleteBookingSVResponse.HasError)
            {
                var bookingApiError = new BookingApiError() 
                { 
                    Detail = deleteBookingSVResponse.Error.Message
                };
                return StatusCode(deleteBookingSVResponse.Error.ErrorCode, bookingApiError);
            }

            //Limpar redis cache caso exista lista e para individual
            var recordKeyList = "*ListBooking_*";
            await redisCache.RemoveRecord(recordKeyList);
            var recordKeyInd = string.Format("*Booking_{0}*", id);
            await redisCache.RemoveRecord(recordKeyInd);

            return StatusCode((int)HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = ex.StackTrace
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }

    public override async Task<IActionResult> GetAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        try
        {
            var getBookingSVRequest = new GetBookingSVRequest()
            {
                Id = Guid.Parse(id)
            };

            var recordKey = string.Format("Booking_{0}", id);
            var getBookingSVResponse = await redisCache.GetRecordAsync<GetBookingSVResponse>(recordKey);
            if (getBookingSVResponse == null)
            {
                getBookingSVResponse = await bookingService.GetAsync(getBookingSVRequest);
                if(getBookingSVResponse.HasError)
                {
                    var bookingApiError = new BookingApiError() 
                    { 
                        Detail = getBookingSVResponse.Error.Message
                    };
                    return StatusCode(getBookingSVResponse.Error.ErrorCode, bookingApiError);
                }

                await redisCache.SetRecordAsync(recordKey, getBookingSVResponse, TimeSpan.FromDays(1));
            }

            var bookingApiResponse = new BookingApiResponse()
            {
                Booking = new()
                {
                    Id = getBookingSVResponse.Id,
                    Name = getBookingSVResponse.Name,
                    BookingDate = getBookingSVResponse.BookingDate,
                    Flexibility = new()
                    {
                        Id = getBookingSVResponse.Flexibility.Id,
                        Description = getBookingSVResponse.Flexibility.Description,
                        Active = getBookingSVResponse.Flexibility.Active,
                        Link = new()
                        {
                            Self = new()
                            {
                                Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = "/Valeting/flexibilities", Id = getBookingSVResponse.Flexibility.Id }).Self
                            }
                        }
                    },
                    VehicleSize = new()
                    {
                        Id = getBookingSVResponse.VehicleSize.Id,
                        Description = getBookingSVResponse.VehicleSize.Description,
                        Active = getBookingSVResponse.VehicleSize.Active,
                        Link = new()
                        {
                            Self = new()
                            {
                                Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = "/Valeting/vehicleSizes", Id = getBookingSVResponse.VehicleSize.Id }).Self
                            }
                        }
                    },
                    ContactNumber = getBookingSVResponse.ContactNumber.Value,
                    Email = getBookingSVResponse.Email,
                    Approved = getBookingSVResponse.Approved,
                    Link = new()
                    {
                        Self = new()
                        {
                            Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = Request.Path.HasValue ? Request.Path.Value : string.Empty }).Self
                        }
                    }
                }
            };
            return StatusCode((int)HttpStatusCode.OK, bookingApiResponse);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = ex.StackTrace
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }

    public override async Task<IActionResult> ListAllAsync([FromQuery] BookingApiParameters bookingApiParameters)
    {
        try
        {
            var paginatedBookingSVRequest = new PaginatedBookingSVRequest()
            {
                Filter = new()
                {
                    PageNumber = bookingApiParameters.PageNumber,
                    PageSize = bookingApiParameters.PageSize
                }
            };

            var recordKey = string.Format("ListBooking_{0}_{1}", bookingApiParameters.PageNumber, bookingApiParameters.PageSize);
            var paginatedBookingSVResponse = await redisCache.GetRecordAsync<PaginatedBookingSVResponse>(recordKey);
            if (paginatedBookingSVResponse == null)
            {
                paginatedBookingSVResponse = await bookingService.ListAllAsync(paginatedBookingSVRequest);
                if(paginatedBookingSVResponse.HasError)
                {
                    var bookingApiError = new BookingApiError() 
                    { 
                        Detail = paginatedBookingSVResponse.Error.Message
                    };
                    return StatusCode(paginatedBookingSVResponse.Error.ErrorCode, bookingApiError);
                }

                await redisCache.SetRecordAsync(recordKey, paginatedBookingSVResponse, TimeSpan.FromMinutes(5));
            }

            var bookingApiPaginatedResponse = new BookingApiPaginatedResponse
            {
                Bookings = [],
                CurrentPage = bookingApiParameters.PageNumber,
                TotalItems = paginatedBookingSVResponse.TotalItems,
                TotalPages = paginatedBookingSVResponse.TotalPages,
                Links = new()
                {
                    Prev = new() { Href = string.Empty },
                    Next = new() { Href = string.Empty },
                    Self = new() { Href = string.Empty }
                }
            };

            var paginatedLinks = urlService.GeneratePaginatedLinks
            (
                new GeneratePaginatedLinksSVRequest()
                {
                    BaseUrl = Request.Host.Value,
                    Path = Request.Path.HasValue ? Request.Path.Value : string.Empty,
                    QueryString = Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty,
                    PageNumber = bookingApiParameters.PageNumber, 
                    TotalPages = paginatedBookingSVResponse.TotalPages,
                    Filter = paginatedBookingSVRequest.Filter
                }
            );

            bookingApiPaginatedResponse.Links.Prev.Href = paginatedLinks.Prev;
            bookingApiPaginatedResponse.Links.Next.Href = paginatedLinks.Next;
            bookingApiPaginatedResponse.Links.Self.Href = paginatedLinks.Self;

            bookingApiPaginatedResponse.Bookings.AddRange(
                paginatedBookingSVResponse.Bookings.Select(item => 
                    new BookingApi()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        BookingDate = item.BookingDate,
                        Flexibility = new()
                        {
                            Id = item.Flexibility.Id,
                            Description = item.Flexibility.Description,
                            Active = item.Flexibility.Active,
                            Link = new()
                            {
                                Self = new()
                                {
                                    Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = "/Valeting/flexibilities", Id = item.Flexibility.Id }).Self
                                }
                            }
                        },
                        VehicleSize = new()
                        {
                            Id = item.VehicleSize.Id,
                            Description = item.VehicleSize.Description,
                            Active = item.VehicleSize.Active,
                            Link = new()
                            {
                                Self = new()
                                {
                                    Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = "/Valeting/vehicleSizes", Id = item.VehicleSize.Id }).Self
                                }
                            }
                        },
                        ContactNumber = item.ContactNumber.Value,
                        Email = item.Email,
                        Approved = item.Approved,
                        Link = new()
                        {
                            Self = new()
                            {
                                Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = Request.Path.Value, Id = item.Id }).Self
                            }
                        }
                    }
                ).ToList()
            );
            return StatusCode((int)HttpStatusCode.OK, bookingApiPaginatedResponse);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = ex.StackTrace
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }
}