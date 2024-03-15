using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.Business.Booking;
using Valeting.Common.Exceptions;
using Valeting.Helpers.Interfaces;
using Valeting.ApiObjects.Booking;
using Valeting.Services.Interfaces;
using Valeting.Controllers.BaseController;
using Valeting.Services.Objects.Booking;

namespace Valeting.Controllers;

public class BookingController(IRedisCache redisCache, IBookingService bookingService, IUrlService urlService) : BookingBaseController
{
    public override async Task<IActionResult> ListAllAsync([FromQuery] BookingApiParameters bookingApiParameters)
    {
        try
        {
            var bookingApiPaginatedResponse = new BookingApiPaginatedResponse()
            {
                Bookings = [],
                CurrentPage = bookingApiParameters.PageNumber,
                Links = new()
                {
                    Prev = new() { Href = string.Empty },
                    Next = new() { Href = string.Empty },
                    Self = new() { Href = string.Empty }
                }
            };

            var bookingFilterDTO = new BookingFilterDTO()
            {
                PageNumber = bookingApiParameters.PageNumber,
                PageSize = bookingApiParameters.PageSize
            };

            var recordKey = string.Format("ListBooking_{0}_{1}", bookingApiParameters.PageNumber, bookingApiParameters.PageSize);

            var bookingListDTO = await redisCache.GetRecordAsync<BookingListDTO>(recordKey);
            if (bookingListDTO == null)
            {
                bookingListDTO = await bookingService.ListAllAsync(bookingFilterDTO);
                await redisCache.SetRecordAsync(recordKey, bookingListDTO, TimeSpan.FromMinutes(5));
            }

            bookingApiPaginatedResponse.TotalItems = bookingListDTO.TotalItems;
            bookingApiPaginatedResponse.TotalPages = bookingListDTO.TotalPages;

            var linkDTO = urlService.GeneratePaginatedLinks
            (
                Request.Host.Value,
                Request.Path.HasValue ? Request.Path.Value : string.Empty,
                Request.QueryString.HasValue ? Request.QueryString.Value : string.Empty,
                bookingApiParameters.PageNumber, bookingListDTO.TotalPages, bookingFilterDTO
            );

            bookingApiPaginatedResponse.Links.Prev.Href = linkDTO.Prev;
            bookingApiPaginatedResponse.Links.Next.Href = linkDTO.Next;
            bookingApiPaginatedResponse.Links.Self.Href = linkDTO.Self;

            bookingApiPaginatedResponse.Bookings.AddRange(
                bookingListDTO.Bookings.Select(item => new BookingApi()
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
                                Href = urlService.GenerateSelf(Request.Host.Value, "/Valeting/flexibilities", item.Flexibility.Id)
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
                                Href = urlService.GenerateSelf(Request.Host.Value, "/Valeting/vehicleSizes", item.VehicleSize.Id)
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
                            Href = urlService.GenerateSelf(Request.Host.Value, Request.Path.Value, item.Id)
                        }
                    }
                }
                ).ToList()
            );

            return StatusCode((int)HttpStatusCode.OK, bookingApiPaginatedResponse);
        }
        catch (InputException inpuException)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = inpuException.Message
            };
            return StatusCode((int)HttpStatusCode.BadRequest, bookingApiError);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }

    public override async Task<IActionResult> CreateAsync([FromBody] CreateBookingApiRequest createBookingApiRequest)
    {
        try
        {
            //Criar middleware para verificar se o request está populado.
            if(createBookingApiRequest == null)
            {
                var bookingApiError = new BookingApiError() 
                { 
                    Detail = "Invalid request body"
                }; 
                return StatusCode((int)HttpStatusCode.BadRequest, bookingApiError);
            }

            var createBookingSVRequest = new CreateBookingSVRequest()
            {
                Name = createBookingApiRequest.Name,
                BookingDate = createBookingApiRequest.BookingDate,
                //Flexibility = createBookingApiRequest.Flexibility != null ? new() { Id = createBookingApiRequest.Flexibility.Id } : null,
                //VehicleSize = createBookingApiRequest.VehicleSize != null ? new() { Id = createBookingApiRequest.VehicleSize.Id } : null,
                ContactNumber = createBookingApiRequest.ContactNumber,
                Email = createBookingApiRequest.Email
            };

            var booking = await bookingService.CreateAsync(createBookingSVRequest);
            if(booking.HasError)
            {
                var bookingApiError = new BookingApiError() 
                { 
                    Detail = booking.Error.Message
                };
                return StatusCode(booking.Error.ErrorCode, bookingApiError);
            }

            //Limpar redis cache caso exista lista de bookings em cache, validar de como ter todas as keys para "List_"
            var recordKey = "*ListBooking_*";
            await redisCache.RemoveRecord(recordKey);

            var createBookingApiResponse = new CreateBookingApiResponse()
            {
                Id = booking.Id
            };

            return StatusCode((int)HttpStatusCode.Created, createBookingApiResponse);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }

    public override async Task<IActionResult> FindByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        try
        {
            var bookingApiResponse = new BookingApiResponse()
            {
                Booking = new()
            };

            var recordKey = string.Format("Booking_{0}", id);

            var bookingDTO = await redisCache.GetRecordAsync<BookingDTO>(recordKey);
            if (bookingDTO == null)
            {
                bookingDTO = await bookingService.FindByIDAsync(Guid.Parse(id));
                await redisCache.SetRecordAsync(recordKey, bookingDTO, TimeSpan.FromDays(1));
            }

            var bookingApi = new BookingApi()
            {
                Id = bookingDTO.Id,
                Name = bookingDTO.Name,
                BookingDate = bookingDTO.BookingDate,
                Flexibility = new()
                {
                    Id = bookingDTO.Flexibility.Id,
                    Description = bookingDTO.Flexibility.Description,
                    Active = bookingDTO.Flexibility.Active,
                    Link = new()
                    {
                        Self = new()
                        {
                            Href = urlService.GenerateSelf(Request.Host.Value, "/Valeting/flexibilities", bookingDTO.Flexibility.Id)
                        }
                    }
                },
                VehicleSize = new()
                {
                    Id = bookingDTO.VehicleSize.Id,
                    Description = bookingDTO.VehicleSize.Description,
                    Active = bookingDTO.VehicleSize.Active,
                    Link = new()
                    {
                        Self = new()
                        {
                            Href = urlService.GenerateSelf(Request.Host.Value, "/Valeting/vehicleSizes", bookingDTO.VehicleSize.Id)
                        }
                    }
                },
                ContactNumber = bookingDTO.ContactNumber.Value,
                Email = bookingDTO.Email,
                Approved = bookingDTO.Approved,
                Link = new()
                {
                    Self = new()
                    {
                        Href = urlService.GenerateSelf(Request.Host.Value, Request.Path.HasValue ? Request.Path.Value : string.Empty)
                    }
                }
            };

            bookingApiResponse.Booking = bookingApi;

            return StatusCode((int)HttpStatusCode.OK, bookingApiResponse);
        }
        catch (InputException inpuException)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = inpuException.Message
            };
            return StatusCode((int)HttpStatusCode.BadRequest, bookingApiError);
        }
        catch (NotFoundException notFoundException)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = notFoundException.Message
            };
            return StatusCode((int)HttpStatusCode.NotFound, bookingApiError);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }

    public override async Task<IActionResult> Update([FromRoute(Name = "id"), MinLength(1), Required] string id, [FromBody] UpdateBookingApiRequest updateBookingApiRequest)
    {
        try
        {
            var bookingDTO = updateBookingApiRequest == null ? null : new BookingDTO()
            {
                Id = Guid.Parse(id),
                Name = updateBookingApiRequest.Name,
                BookingDate = updateBookingApiRequest.BookingDate,
                Flexibility = updateBookingApiRequest.Flexibility != null ? new() { Id = updateBookingApiRequest.Flexibility.Id } : null,
                VehicleSize = updateBookingApiRequest.VehicleSize != null ? new() { Id = updateBookingApiRequest.VehicleSize.Id } : null,
                ContactNumber = updateBookingApiRequest.ContactNumber,
                Email = updateBookingApiRequest.Email,
                Approved = updateBookingApiRequest.Approved
            };

            await bookingService.UpdateAsync(bookingDTO);

            //Limpar redis cache caso exista lista e para individual
            var recordKeyList = "*ListBooking_*";
            await redisCache.RemoveRecord(recordKeyList);
            var recordKeyInd = string.Format("*Booking_{0}*", id);
            await redisCache.RemoveRecord(recordKeyInd);

            return StatusCode((int)HttpStatusCode.NoContent);
        }
        catch (BusinessObjectException businessObjectException)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = businessObjectException.Message
            };
            return StatusCode((int)HttpStatusCode.BadRequest, bookingApiError);
        }
        catch (InputException inpuException)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = inpuException.Message
            };
            return StatusCode((int)HttpStatusCode.BadRequest, bookingApiError);
        }
        catch (NotFoundException notFoundException)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = notFoundException.Message
            };
            return StatusCode((int)HttpStatusCode.NotFound, bookingApiError);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }

    public override async Task<IActionResult> Delete([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        try
        {
            await bookingService.DeleteAsync(Guid.Parse(id));
            //Limpar redis cache caso exista lista e para individual
            var recordKeyList = "*ListBooking_*";
            await redisCache.RemoveRecord(recordKeyList);
            var recordKeyInd = string.Format("*Booking_{0}*", id);
            await redisCache.RemoveRecord(recordKeyInd);

            return StatusCode((int)HttpStatusCode.NoContent);
        }
        catch (InputException inpuException)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = inpuException.Message
            };
            return StatusCode((int)HttpStatusCode.BadRequest, bookingApiError);
        }
        catch (NotFoundException notFoundException)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = notFoundException.Message
            };
            return StatusCode((int)HttpStatusCode.NotFound, bookingApiError);
        }
        catch (Exception ex)
        {
            var bookingApiError = new BookingApiError() 
            { 
                Detail = ex.Message
            };
            return StatusCode((int)HttpStatusCode.InternalServerError, bookingApiError);
        }
    }
}