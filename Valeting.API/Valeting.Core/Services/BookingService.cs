using System.Net;
using Valeting.Core.Interfaces;
using Valeting.Common.Messages;
using Valeting.Services.Validators;
using Valeting.Common.Models.Booking;
using Valeting.Repository.Interfaces;

namespace Valeting.Core.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    public async Task<CreateBookingDtoResponse> CreateAsync(CreateBookingDtoRequest createBookingDtoRequest)
    {
        var createBookingDtoResponse = new CreateBookingDtoResponse();
        var validator = new CreateBookingValidator();
        var result = validator.Validate(createBookingDtoRequest);
        if (!result.IsValid)
        {
            createBookingDtoResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return createBookingDtoResponse;
        }

        var id = Guid.NewGuid();
        var bookingDto = new BookingDto()
        {
            Id = id,
            Name = createBookingDtoRequest.Name,
            Email = createBookingDtoRequest.Email,
            ContactNumber = createBookingDtoRequest.ContactNumber,
            BookingDate = createBookingDtoRequest.BookingDate,
            Flexibility = new() { Id = createBookingDtoRequest.Flexibility.Id },
            VehicleSize = new() { Id = createBookingDtoRequest.VehicleSize.Id },
            Approved = false
        };
        await bookingRepository.CreateAsync(bookingDto);
        
        createBookingDtoResponse.Id = id;
        return createBookingDtoResponse;
    }

    public async Task<UpdateBookingDtoResponse> UpdateAsync(UpdateBookingDtoRequest updateBookingDtoRequest)
    {
        var updateBookingDtoResponse = new UpdateBookingDtoResponse();
        var validator = new UpdateBookinValidator();
        var result = await validator.ValidateAsync(updateBookingDtoRequest);
        if(!result.IsValid)
        {
            updateBookingDtoResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return updateBookingDtoResponse;
        }

        var bookingDto = await bookingRepository.GetByIdAsync(updateBookingDtoRequest.Id);
        if (bookingDto == null)
        {
            updateBookingDtoResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.BookingNotFound
            };
            return updateBookingDtoResponse;
        }

        bookingDto.Id = updateBookingDtoRequest.Id;
        bookingDto.Name = updateBookingDtoRequest.Name;
        bookingDto.BookingDate = updateBookingDtoRequest.BookingDate;
        bookingDto.Flexibility = updateBookingDtoRequest.Flexibility != null ? new() { Id = updateBookingDtoRequest.Flexibility.Id } : null;
        bookingDto.VehicleSize = updateBookingDtoRequest.VehicleSize != null ? new() { Id = updateBookingDtoRequest.VehicleSize.Id } : null;
        bookingDto.ContactNumber = updateBookingDtoRequest.ContactNumber;
        bookingDto.Email = updateBookingDtoRequest.Email;
        bookingDto.Approved = updateBookingDtoRequest.Approved;
        await bookingRepository.UpdateAsync(bookingDto);

        return updateBookingDtoResponse;
    }

    public async Task<DeleteBookingDtoResponse> DeleteAsync(DeleteBookingDtoRequest deleteBookingDtoRequest)
    {
        var deleteBookingDtoResponse = new DeleteBookingDtoResponse();
        var validator = new DeleteBookingValidator();
        var result = validator.Validate(deleteBookingDtoRequest);
        if(!result.IsValid)
        {
            deleteBookingDtoResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return deleteBookingDtoResponse;
        }

        var bookingDto = await bookingRepository.GetByIdAsync(deleteBookingDtoRequest.Id);
        if (bookingDto == null)
        {
            deleteBookingDtoResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.BookingNotFound
            };
            return deleteBookingDtoResponse;
        }

        await bookingRepository.DeleteAsync(deleteBookingDtoRequest.Id);

        return deleteBookingDtoResponse;
    }

    public async Task<GetBookingDtoResponse> GetByIdAsync(GetBookingDtoRequest getBookingDtoRequest)
    {
        var getBookingDtoResponse = new GetBookingDtoResponse();

        var validator = new GetBookingValidator();
        var result = validator.Validate(getBookingDtoRequest);
        if(!result.IsValid)
        {
            getBookingDtoResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return getBookingDtoResponse;
        }

        var bookingDto = await bookingRepository.GetByIdAsync(getBookingDtoRequest.Id);
        if (bookingDto == null)
        {
            getBookingDtoResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.BookingNotFound
            };
            return getBookingDtoResponse;
        }

        getBookingDtoResponse.Id = bookingDto.Id;
        getBookingDtoResponse.Name = bookingDto.Name;
        getBookingDtoResponse.BookingDate = bookingDto.BookingDate;
        getBookingDtoResponse.ContactNumber = bookingDto.ContactNumber;
        getBookingDtoResponse.Flexibility = new() { Id = bookingDto.Flexibility.Id, Description = bookingDto.Flexibility.Description, Active = bookingDto.Flexibility.Active };
        getBookingDtoResponse.VehicleSize = new() { Id = bookingDto.VehicleSize.Id, Description = bookingDto.VehicleSize.Description, Active = bookingDto.VehicleSize.Active };
        getBookingDtoResponse.Email = bookingDto.Email;
        getBookingDtoResponse.Approved = bookingDto.Approved;
        return getBookingDtoResponse;
    }

    public async Task<PaginatedBookingDtoResponse> GetAsync(PaginatedBookingDtoRequest paginatedBookingDtoRequest)
    {
        var paginatedBookingDtoResponse = new PaginatedBookingDtoResponse();
        
        var validator = new PaginatedBookingValidator();
        var result = validator.Validate(paginatedBookingDtoRequest);
        if(!result.IsValid)
        {
            paginatedBookingDtoResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.BadRequest,
                Message = result.Errors.FirstOrDefault().ErrorMessage
            };
            return paginatedBookingDtoResponse;
        }

        var bookingFilterDto = new BookingFilterDto()
        {
            PageNumber = paginatedBookingDtoRequest.Filter.PageNumber,
            PageSize = paginatedBookingDtoRequest.Filter.PageSize
        };

        var bookingListDto = await bookingRepository.GetAsync(bookingFilterDto);
        if(bookingListDto == null)
        {
            paginatedBookingDtoResponse.Error = new()
            {
                ErrorCode = (int)HttpStatusCode.NotFound,
                Message = Messages.BookingNotFound
            };
            return paginatedBookingDtoResponse;
        }

        paginatedBookingDtoResponse.TotalItems = bookingListDto.TotalItems;
        paginatedBookingDtoResponse.TotalPages = bookingListDto.TotalPages;

        paginatedBookingDtoResponse.Bookings = bookingListDto.Bookings.Select(x => 
            new BookingDto()
            {
                Id = x.Id,
                Name = x.Name,
                BookingDate = x.BookingDate,
                ContactNumber = x.ContactNumber,
                Flexibility = new() { Id = x.Flexibility.Id, Description = x.Flexibility.Description, Active = x.Flexibility.Active},
                VehicleSize = new() { Id = x.VehicleSize.Id, Description = x.VehicleSize.Description, Active = x.VehicleSize.Active},
                Email = x.Email,
                Approved = x.Approved
            }
        ).ToList();

        return paginatedBookingDtoResponse;
    }
}