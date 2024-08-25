using AutoMapper;

using System.Net;

using Valeting.Common.Messages;
using Valeting.Core.Validators;
using Valeting.Core.Models.Booking;
using Valeting.Core.Validators.Helper;
using Valeting.Core.Services.Interfaces;
using Valeting.Repository.Models.Booking;
using Valeting.Repository.Repositories.Interfaces;

namespace Valeting.Core.Services;

public class BookingService(IBookingRepository bookingRepository, ValidationHelpers validationHelpers, IMapper mapper) : IBookingService
{
    public async Task<CreateBookingSVResponse> CreateAsync(CreateBookingSVRequest createBookingSVRequest)
    {
        var createBookingSVResponse = new CreateBookingSVResponse();
        var validator = new CreateBookingValidator(validationHelpers);
        var result = await validator.ValidateAsync(createBookingSVRequest);
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

        var bookingDTO = mapper.Map<BookingDTO>(createBookingSVRequest);
        bookingDTO.Id = id;

        await bookingRepository.CreateAsync(bookingDTO);
        
        createBookingSVResponse.Id = id;
        return createBookingSVResponse;
    }

    public async Task<UpdateBookingSVResponse> UpdateAsync(UpdateBookingSVRequest updateBookingSVRequest)
    {
        var updateBookingSVResponse = new UpdateBookingSVResponse();
        var validator = new UpdateBookinValidator(validationHelpers);
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

        var bookingDTO = await bookingRepository.GetByIdAsync(updateBookingSVRequest.Id);
        if (bookingDTO == null)
        {
            updateBookingSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.BookingNotFound
            };
            return updateBookingSVResponse;
        }

        mapper.Map(updateBookingSVRequest, bookingDTO);
        await bookingRepository.UpdateAsync(bookingDTO);

        return updateBookingSVResponse;
    }

    public async Task<DeleteBookingSVResponse> DeleteAsync(DeleteBookingSVRequest deleteBookingSVRequest)
    {
        var deleteBookingSVResponse = new DeleteBookingSVResponse();
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

        var bookingDTO = await bookingRepository.GetByIdAsync(deleteBookingSVRequest.Id);
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

    public async Task<PaginatedBookingSVResponse> GetAsync(PaginatedBookingSVRequest paginatedBookingSVRequest)
    {
        var paginatedBookingSVResponse = new PaginatedBookingSVResponse();
        
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

        var bookingFilterDTO = mapper.Map<BookingFilterDTO>(paginatedBookingSVRequest.Filter);

        var bookingListDTO = await bookingRepository.GetAsync(bookingFilterDTO);
        if(bookingListDTO == null)
        {
            paginatedBookingSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.BookingNotFound
            };
            return paginatedBookingSVResponse;
        }

        return mapper.Map<PaginatedBookingSVResponse>(bookingListDTO);
    }

    public async Task<GetBookingSVResponse> GetByIdAsync(GetBookingSVRequest getBookingSVRequest)
    {
        var getBookingSVResponse = new GetBookingSVResponse();

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

        var bookingDTO = await bookingRepository.GetByIdAsync(getBookingSVRequest.Id);
        if (bookingDTO == null)
        {
            getBookingSVResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.BookingNotFound
            };
            return getBookingSVResponse;
        }

        getBookingSVResponse.Booking = mapper.Map<BookingSV>(bookingDTO);
        return getBookingSVResponse;
    }
}