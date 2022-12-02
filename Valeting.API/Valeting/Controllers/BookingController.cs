﻿using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.ApiObjects;
using Valeting.Business.Booking;
using Valeting.Common.Exceptions;
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
        private IRedisCache _redisCache;
        private readonly IUrlService _urlService;
        private readonly IBookingService _bookingService;
        private BookingApiError _bookingApiError;

        public BookingController(IRedisCache redisCache, IBookingService bookingService, IUrlService urlService)
        {
            _redisCache = redisCache;
            _bookingService = bookingService;
            _urlService = urlService;
            _bookingApiError = new BookingApiError() { Id = Guid.NewGuid() };
        }

        public override async Task<IActionResult> CreateAsync([FromBody] CreateBookingApiRequest createBookingApiRequest)
        {
            try
            {
                var bookingDTO = createBookingApiRequest == null ? null : new BookingDTO()
                {
                    Name = createBookingApiRequest.Name,
                    BookingDate = createBookingApiRequest.BookingDate,
                    Flexibility = createBookingApiRequest.Flexibility != null ? new FlexibilityDTO() { Id = createBookingApiRequest.Flexibility.Id } : null,
                    VehicleSize = createBookingApiRequest.VehicleSize != null ? new VehicleSizeDTO() { Id = createBookingApiRequest.VehicleSize.Id } : null,
                    ContactNumber = createBookingApiRequest.ContactNumber,
                    Email = createBookingApiRequest.Email
                };

                var booking = await _bookingService.CreateAsync(bookingDTO);

                //Limpar redis cache caso exista lista de bookings em cache, validar de como ter todas as keys para "List_"
                var recordKey = "*ListBooking_*";
                await _redisCache.RemoveRecordAsync(recordKey);

                var createBookingApiResponse = new CreateBookingApiResponse()
                {
                    Id = booking.Id
                };

                return StatusCode((int)HttpStatusCode.Created, createBookingApiResponse);
            }
            catch (BusinessObjectException businessObjectException)
            {
                _bookingApiError.Detail = businessObjectException.Message;
                return StatusCode((int)HttpStatusCode.BadRequest, _bookingApiError);
            }
            catch (InputException inpuException)
            {
                _bookingApiError.Detail = inpuException.Message;
                return StatusCode((int)HttpStatusCode.BadRequest, _bookingApiError);
            }
            catch (Exception ex)
            {
                _bookingApiError.Detail = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, _bookingApiError);
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
                    Flexibility = updateBookingApiRequest.Flexibility != null ? new FlexibilityDTO() { Id = updateBookingApiRequest.Flexibility.Id } : null,
                    VehicleSize = updateBookingApiRequest.VehicleSize != null ? new VehicleSizeDTO() { Id = updateBookingApiRequest.VehicleSize.Id } : null,
                    ContactNumber = updateBookingApiRequest.ContactNumber,
                    Email = updateBookingApiRequest.Email,
                    Approved = updateBookingApiRequest.Approved
                };

                await _bookingService.UpdateAsync(bookingDTO);

                //Limpar redis cache caso exista lista e para individual
                var recordKeyList = "*ListBooking_*";
                await _redisCache.RemoveRecordAsync(recordKeyList);
                var recordKeyInd = string.Format("*Booking_{0}*", id);
                await _redisCache.RemoveRecordAsync(recordKeyInd);

                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (BusinessObjectException businessObjectException)
            {
                _bookingApiError.Detail = businessObjectException.Message;
                return StatusCode((int)HttpStatusCode.BadRequest, _bookingApiError);
            }
            catch (InputException inpuException)
            {
                _bookingApiError.Detail = inpuException.Message;
                return StatusCode((int)HttpStatusCode.BadRequest, _bookingApiError);
            }
            catch (NotFoundException notFoundException)
            {
                _bookingApiError.Detail = notFoundException.Message;
                return StatusCode((int)HttpStatusCode.NotFound, _bookingApiError);
            }
            catch (Exception ex)
            {
                _bookingApiError.Detail = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, _bookingApiError);
            }
        }

        public override async Task<IActionResult> Delete([FromRoute(Name = "id"), MinLength(1), Required] string id)
        {
            try
            {
                await _bookingService.DeleteAsync(Guid.Parse(id));
                //Limpar redis cache caso exista lista e para individual
                var recordKeyList = "*ListBooking_*";
                await _redisCache.RemoveRecordAsync(recordKeyList);
                var recordKeyInd = string.Format("*Booking_{0}*", id);
                await _redisCache.RemoveRecordAsync(recordKeyInd);

                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (InputException inpuException)
            {
                _bookingApiError.Detail = inpuException.Message;
                return StatusCode((int)HttpStatusCode.BadRequest, _bookingApiError);
            }
            catch (NotFoundException notFoundException)
            {
                _bookingApiError.Detail = notFoundException.Message;
                return StatusCode((int)HttpStatusCode.NotFound, _bookingApiError);
            }
            catch (Exception ex)
            {
                _bookingApiError.Detail = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, _bookingApiError);
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

                var bookingDTO = await _redisCache.GetRecordAsync<BookingDTO>(recordKey);
                if(bookingDTO == null)
                {
                    bookingDTO = await _bookingService.FindByIDAsync(Guid.Parse(id));
                    await _redisCache.SetRecordAsync<BookingDTO>(recordKey, bookingDTO, TimeSpan.FromDays(1));
                }

                var bookingApi = new BookingApi()
                {
                    Id = bookingDTO.Id,
                    Name = bookingDTO.Name,
                    BookingDate = bookingDTO.BookingDate,
                    Flexibility = new FlexibilityApi()
                    {
                        Id = bookingDTO.Flexibility.Id,
                        Description = bookingDTO.Flexibility.Description,
                        Active = bookingDTO.Flexibility.Active,
                        Link = new FlexibilityApiLink()
                        {
                            Self = new LinkApi()
                            {
                                Href = _urlService.GenerateSelf(Request.Host.Value, "/Valeting/flexibilities", bookingDTO.Flexibility.Id)
                            }
                        }
                    },
                    VehicleSize = new VehicleSizeApi()
                    {
                        Id = bookingDTO.VehicleSize.Id,
                        Description = bookingDTO.VehicleSize.Description,
                        Active = bookingDTO.VehicleSize.Active,
                        Link = new VehicleSizeApiLink()
                        {
                            Self = new LinkApi()
                            {
                                Href = _urlService.GenerateSelf(Request.Host.Value, "/Valeting/vehicleSizes", bookingDTO.VehicleSize.Id)
                            }
                        }
                    },
                    ContactNumber = bookingDTO.ContactNumber.Value,
                    Email = bookingDTO.Email,
                    Approved = bookingDTO.Approved,
                    Link = new BookingApiLink()
                    {
                        Self = new LinkApi()
                        {
                            Href = _urlService.GenerateSelf(Request.Host.Value, Request.Path.HasValue ? Request.Path.Value : string.Empty)
                        }
                    }
                };

                bookingApiResponse.Booking = bookingApi;

                return StatusCode((int)HttpStatusCode.OK, bookingApiResponse);
            }
            catch (InputException inpuException)
            {
                _bookingApiError.Detail = inpuException.Message;
                return StatusCode((int)HttpStatusCode.BadRequest, _bookingApiError);
            }
            catch (NotFoundException notFoundException)
            {
                _bookingApiError.Detail = notFoundException.Message;
                return StatusCode((int)HttpStatusCode.NotFound, _bookingApiError);
            }
            catch (Exception ex)
            {
                _bookingApiError.Detail = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, _bookingApiError);
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
                    {
                        Prev = new LinkApi() { Href = string.Empty },
                        Next = new LinkApi() { Href = string.Empty },
                        Self = new LinkApi() { Href = string.Empty }
                    }
                };

                var bookingFilterDTO = new BookingFilterDTO()
                {
                    PageNumber = bookingApiParameters.PageNumber,
                    PageSize = bookingApiParameters.PageSize
                };

                var recordKey = string.Format("ListBooking_{0}_{1}", bookingApiParameters.PageNumber, bookingApiParameters.PageSize);

                var bookingListDTO = await _redisCache.GetRecordAsync<BookingListDTO>(recordKey);
                if (bookingListDTO == null)
                {
                    bookingListDTO = await _bookingService.ListAllAsync(bookingFilterDTO);
                    await _redisCache.SetRecordAsync<BookingListDTO>(recordKey, bookingListDTO, TimeSpan.FromMinutes(5));
                }

                bookingApiPaginatedResponse.TotalItems = bookingListDTO.TotalItems;
                bookingApiPaginatedResponse.TotalPages = bookingListDTO.TotalPages;

                var linkDTO = _urlService.GeneratePaginatedLinks
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
                            Flexibility = new FlexibilityApi()
                            {
                                Id = item.Flexibility.Id,
                                Description = item.Flexibility.Description,
                                Active = item.Flexibility.Active,
                                Link = new FlexibilityApiLink()
                                {
                                    Self = new LinkApi()
                                    {
                                        Href = _urlService.GenerateSelf(Request.Host.Value, "/Valeting/flexibilities", item.Flexibility.Id)
                                    }
                                }
                            },
                            VehicleSize = new VehicleSizeApi()
                            {
                                Id = item.VehicleSize.Id,
                                Description = item.VehicleSize.Description,
                                Active = item.VehicleSize.Active,
                                Link = new VehicleSizeApiLink()
                                {
                                    Self = new LinkApi()
                                    {
                                        Href = _urlService.GenerateSelf(Request.Host.Value, "/Valeting/vehicleSizes", item.VehicleSize.Id)
                                    }
                                }
                            },
                            ContactNumber = item.ContactNumber.Value,
                            Email = item.Email,
                            Approved = item.Approved,
                            Link = new BookingApiLink()
                            {
                                Self = new LinkApi()
                                {
                                    Href = _urlService.GenerateSelf(Request.Host.Value, Request.Path.Value, item.Id)
                                }
                            }
                        }
                    ).ToList()
                );

                return StatusCode((int)HttpStatusCode.OK, bookingApiPaginatedResponse);
            }
            catch (InputException inpuException)
            {
                _bookingApiError.Detail = inpuException.Message;
                return StatusCode((int)HttpStatusCode.BadRequest, _bookingApiError);
            }
            catch (Exception ex)
            {
                _bookingApiError.Detail = ex.Message;
                return StatusCode((int)HttpStatusCode.InternalServerError, _bookingApiError);
            }
        }
    }
}
