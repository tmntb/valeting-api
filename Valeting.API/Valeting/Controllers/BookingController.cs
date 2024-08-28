﻿using AutoMapper;

using System.Net;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Valeting.Models.Core;
using Valeting.Models.Booking;
using Valeting.Cache.Interfaces;
using Valeting.Core.Models.Link;
using Valeting.Core.Models.Booking;
using Valeting.Core.Services.Interfaces;
using Valeting.Controllers.BaseController;

namespace Valeting.Controllers;

public class BookingController(IBookingService bookingService, IUrlService urlService, ICacheHandler cacheHandler, IMapper mapper) : BookingBaseController
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

            var createBookingSVRequest = mapper.Map<CreateBookingSVRequest>(createBookingApiRequest);
            var createBookingSVResponse = await bookingService.CreateAsync(createBookingSVRequest);
            if(createBookingSVResponse.HasError)
            {
                var bookingApiError = new BookingApiError() 
                { 
                    Detail = createBookingSVResponse.Error.Message
                };
                return StatusCode(createBookingSVResponse.Error.ErrorCode, bookingApiError);
            }

            var recordKey = "ListBooking_";
            cacheHandler.RemoveRecordsWithPrefix(recordKey);

            var createBookingApiResponse = mapper.Map<CreateBookingApiResponse>(createBookingSVResponse);
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

    public override async Task<IActionResult> UpdateAsync([FromRoute(Name = "id"), MinLength(1), Required] string id, [FromBody] UpdateBookingApiRequest updateBookingApiRequest)
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

            var updateBookingSVRequest = mapper.Map<UpdateBookingSVRequest>(updateBookingApiRequest);
            updateBookingSVRequest.Id = Guid.Parse(id);

            var updateBookingSVResponse = await bookingService.UpdateAsync(updateBookingSVRequest);
            if(updateBookingSVResponse.HasError)
            {
                var bookingApiError = new BookingApiError() 
                { 
                    Detail = updateBookingSVResponse.Error.Message
                };
                return StatusCode(updateBookingSVResponse.Error.ErrorCode, bookingApiError);
            }

            var recordKeyList = "ListBooking_";
            cacheHandler.RemoveRecordsWithPrefix(recordKeyList);
            var recordKeyId = string.Format("Booking_{0}", id);
            cacheHandler.RemoveRecord(recordKeyId);

            return StatusCode((int)HttpStatusCode.NoContent);
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

    public override async Task<IActionResult> DeleteAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
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

            var recordKeyList = "ListBooking_";
            cacheHandler.RemoveRecordsWithPrefix(recordKeyList);
            var recordKeyId = string.Format("Booking_{0}", id);
            cacheHandler.RemoveRecord(recordKeyId);

            return StatusCode((int)HttpStatusCode.NoContent);
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

    public override async Task<IActionResult> GetByIdAsync([FromRoute(Name = "id"), MinLength(1), Required] string id)
    {
        try
        {
            var getBookingSVRequest = new GetBookingSVRequest()
            {
                Id = Guid.Parse(id)
            };

            var recordKey = string.Format("Booking_{0}", id);
            var getBookingSVResponse = cacheHandler.GetRecord<GetBookingSVResponse>(recordKey);
            if (getBookingSVResponse == null)
            {
                getBookingSVResponse = await bookingService.GetByIdAsync(getBookingSVRequest);
                if(getBookingSVResponse.HasError)
                {
                    var bookingApiError = new BookingApiError() 
                    { 
                        Detail = getBookingSVResponse.Error.Message
                    };
                    return StatusCode(getBookingSVResponse.Error.ErrorCode, bookingApiError);
                }

                cacheHandler.SetRecord(recordKey, getBookingSVResponse, TimeSpan.FromDays(1));
            }

            var bookingApi = mapper.Map<BookingApi>(getBookingSVResponse.Booking);
            bookingApi.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = Request.Path.HasValue ? Request.Path.Value : string.Empty }).Self
                }
            };
            bookingApi.Flexibility.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = "/flexibilities", Id = bookingApi.Flexibility.Id }).Self
                }
            };
            bookingApi.VehicleSize.Link = new()
            {
                Self = new()
                {
                    Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = "/vehicleSizes", Id = bookingApi.VehicleSize.Id }).Self
                }
            };

            var bookingApiResponse = new BookingApiResponse()
            {
                Booking = bookingApi
            };
            return StatusCode((int)HttpStatusCode.OK, bookingApiResponse);
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

    public override async Task<IActionResult> GetAsync([FromQuery] BookingApiParameters bookingApiParameters)
    {
        try
        {
            var paginatedBookingSVRequest = mapper.Map<PaginatedBookingSVRequest>(bookingApiParameters);

            var recordKey = string.Format("ListBooking_{0}_{1}", bookingApiParameters.PageNumber, bookingApiParameters.PageSize);
            var paginatedBookingSVResponse = cacheHandler.GetRecord<PaginatedBookingSVResponse>(recordKey);
            if (paginatedBookingSVResponse == null)
            {
                paginatedBookingSVResponse = await bookingService.GetAsync(paginatedBookingSVRequest);
                if(paginatedBookingSVResponse.HasError)
                {
                    var bookingApiError = new BookingApiError() 
                    { 
                        Detail = paginatedBookingSVResponse.Error.Message
                    };
                    return StatusCode(paginatedBookingSVResponse.Error.ErrorCode, bookingApiError);
                }

                cacheHandler.SetRecord(recordKey, paginatedBookingSVResponse, TimeSpan.FromMinutes(5));
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

            var links = mapper.Map<PaginationLinksApi>(paginatedLinks);
            bookingApiPaginatedResponse.Links = links;

            var bookingApis = mapper.Map<List<BookingApi>>(paginatedBookingSVResponse.Bookings);
            bookingApis.ForEach(b => 
            {
                b.Flexibility.Link = new()
                {
                    Self = new()
                    {
                        Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = "/flexibilities", Id = b.Flexibility.Id }).Self
                    }
                };

                b.VehicleSize.Link = new()
                {
                    Self = new()
                    {
                        Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = "/vehicleSizes", Id = b.VehicleSize.Id }).Self
                    }
                };

                b.Link = new()
                {
                    Self = new()
                    {
                        Href = urlService.GenerateSelf(new GenerateSelfUrlSVRequest() { BaseUrl = Request.Host.Value, Path = Request.Path.Value, Id = b.Id }).Self
                    }
                };
            });
            bookingApiPaginatedResponse.Bookings = bookingApis;

            return StatusCode((int)HttpStatusCode.OK, bookingApiPaginatedResponse);
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