﻿using System.Net;

using Valeting.Common.Messages;
using Valeting.Business.Booking;
using Valeting.Services.Validators;
using Valeting.Services.Interfaces;
using Valeting.Repositories.Interfaces;
using Valeting.Services.Objects.Booking;

namespace Valeting.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    public async Task<CreateBookingSVResponse> CreateAsync(CreateBookingSVRequest createBookingSVRequest)
    {
        var createBookingSVResponse = new CreateBookingSVResponse() { Error = new() };
        var validator = new CreateBookingValidator();
        var result = validator.Validate(createBookingSVRequest);
        if (!result.IsValid)
        {
            createBookingSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return createBookingSVResponse;
        }

        var id = Guid.NewGuid();
        var bookingDTO = new BookingDTO()
        {
            Id = id,
            Name = createBookingSVRequest.Name,
            Email = createBookingSVRequest.Email,
            ContactNumber = createBookingSVRequest.ContactNumber,
            BookingDate = createBookingSVRequest.BookingDate
        };
        await bookingRepository.CreateAsync(bookingDTO);
        
        createBookingSVResponse.Id = id;
        return createBookingSVResponse;
    }

    public async Task<UpdateBookingSVResponse> UpdateAsync(UpdateBookingSVRequest updateBookingSVRequest)
    {
        var updateBookingSVResponse = new UpdateBookingSVResponse() { Error = new() };
        var validator = new UpdateBookinValidator();
        var result = await validator.ValidateAsync(updateBookingSVRequest);
        if(!result.IsValid)
        {
            updateBookingSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return updateBookingSVResponse;
        }

        var bookingDTO = await bookingRepository.FindByIdAsync(updateBookingSVRequest.Id);
        if (bookingDTO == null)
        {
            updateBookingSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.BookingNotFound
            };
            return updateBookingSVResponse;
        }

        bookingDTO.Id = updateBookingSVRequest.Id;
        bookingDTO.Name = updateBookingSVRequest.Name;
        bookingDTO.BookingDate = updateBookingSVRequest.BookingDate;
        //bookingDTO.Flexibility = updateBookingApiRequest.Flexibility != null ? new() { Id = updateBookingApiRequest.Flexibility.Id } : null,
        //bookingDTO.VehicleSize = updateBookingApiRequest.VehicleSize != null ? new() { Id = updateBookingApiRequest.VehicleSize.Id } : null,
        bookingDTO.ContactNumber = updateBookingSVRequest.ContactNumber;
        bookingDTO.Email = updateBookingSVRequest.Email;
        bookingDTO.Approved = updateBookingSVRequest.Approved;
        await bookingRepository.UpdateAsync(bookingDTO);

        return updateBookingSVResponse;
    }

    public async Task<DeleteBookingSVResponse> DeleteAsync(DeleteBookingSVRequest deleteBookingSVRequest)
    {
        var deleteBookingSVResponse = new DeleteBookingSVResponse() { Error = new() };
        var validator = new DeleteBookingValidator();
        var result = validator.Validate(deleteBookingSVRequest);
        if(!result.IsValid)
        {
            deleteBookingSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return deleteBookingSVResponse;
        }

        var bookingDTO = await bookingRepository.FindByIdAsync(deleteBookingSVRequest.Id);
        if (bookingDTO == null)
        {
            deleteBookingSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.BookingNotFound
            };
            return deleteBookingSVResponse;
        }

        await bookingRepository.DeleteAsync(deleteBookingSVRequest.Id);

        return deleteBookingSVResponse;
    }

    public async Task<GetBookingSVResponse> GetAsync(GetBookingSVRequest getBookingSVRequest)
    {
        var getBookingSVResponse = new GetBookingSVResponse() { Error = new() };

        var validator = new GetBookingValidator();
        var result = validator.Validate(getBookingSVRequest);
        if(!result.IsValid)
        {
            getBookingSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return getBookingSVResponse;
        }

        var bookingDTO = await bookingRepository.FindByIdAsync(getBookingSVRequest.Id);
        if (bookingDTO == null)
        {
            getBookingSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.BookingNotFound
            };
            return getBookingSVResponse;
        }

        getBookingSVResponse.Id = bookingDTO.Id;
        getBookingSVResponse.Name = bookingDTO.Name;
        getBookingSVResponse.BookingDate = bookingDTO.BookingDate;
        getBookingSVResponse.ContactNumber = bookingDTO.ContactNumber;
        getBookingSVResponse.Email = bookingDTO.Email;
        getBookingSVResponse.Approved = bookingDTO.Approved;
        return getBookingSVResponse;
    }

    public async Task<PaginatedBookingSVResponse> ListAllAsync(PaginatedBookingSVRequest paginatedBookingSVRequest)
    {
        var paginatedBookingSVResponse = new PaginatedBookingSVResponse() { Error = new() };
        
        var validator = new PaginatedBookingValidator();
        var result = validator.Validate(paginatedBookingSVRequest);
        if(!result.IsValid)
        {
            paginatedBookingSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return paginatedBookingSVResponse;
        }

        var bookingFilterDTO = new BookingFilterDTO()
        {
            PageNumber = paginatedBookingSVRequest.Filter.PageNumber,
            PageSize = paginatedBookingSVRequest.Filter.PageSize
        };

        var bookingListDTO = await bookingRepository.ListAsync(bookingFilterDTO);
        if(bookingListDTO == null)
        {
            paginatedBookingSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.BookingNotFound
            };
            return paginatedBookingSVResponse;
        }

        paginatedBookingSVResponse.TotalItems = bookingListDTO.TotalItems;
        paginatedBookingSVResponse.TotalPages = bookingListDTO.TotalPages;

        paginatedBookingSVResponse.Bookings = bookingListDTO.Bookings.Select(x => 
            new BookingSV()
            {
                Id = x.Id,
                Name = x.Name,
                BookingDate = x.BookingDate,
                ContactNumber = x.ContactNumber,
                Email = x.Email,
                Approved = x.Approved
            }
        ).ToList();

        return paginatedBookingSVResponse;
    }
}