using FluentValidation;
using Valeting.Common.Cache;
using Valeting.Core.Interfaces;
using Valeting.Common.Messages;
using Valeting.Services.Validators;
using Valeting.Common.Models.Booking;
using Valeting.Repository.Interfaces;
using Valeting.Common.Cache.Interfaces;

namespace Valeting.Core.Services;

public class BookingService(IBookingRepository bookingRepository, ICacheHandler cacheHandler) : IBookingService
{
    public async Task<CreateBookingDtoResponse> CreateAsync(CreateBookingDtoRequest createBookingDtoRequest)
    {
        var createBookingDtoResponse = new CreateBookingDtoResponse();
        var validator = new CreateBookingValidator();
        var result = validator.Validate(createBookingDtoRequest);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
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

        cacheHandler.InvalidateCacheByListType(CacheListType.Booking);

        return createBookingDtoResponse;
    }

    public async Task<UpdateBookingDtoResponse> UpdateAsync(UpdateBookingDtoRequest updateBookingDtoRequest)
    {
        var updateBookingDtoResponse = new UpdateBookingDtoResponse();
        var validator = new UpdateBookinValidator();
        var result = await validator.ValidateAsync(updateBookingDtoRequest);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        var bookingDto = await bookingRepository.GetByIdAsync(updateBookingDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.BookingNotFound);

        //ver como fazer override dos valores de BookingDto com os de UpdateBookingDtoRequest
        bookingDto.Id = updateBookingDtoRequest.Id;
        bookingDto.Name = updateBookingDtoRequest.Name;
        bookingDto.BookingDate = updateBookingDtoRequest.BookingDate;
        bookingDto.Flexibility = updateBookingDtoRequest.Flexibility != null ? new() { Id = updateBookingDtoRequest.Flexibility.Id } : null;
        bookingDto.VehicleSize = updateBookingDtoRequest.VehicleSize != null ? new() { Id = updateBookingDtoRequest.VehicleSize.Id } : null;
        bookingDto.ContactNumber = updateBookingDtoRequest.ContactNumber;
        bookingDto.Email = updateBookingDtoRequest.Email;
        bookingDto.Approved = updateBookingDtoRequest.Approved;
        await bookingRepository.UpdateAsync(bookingDto);

        // Keep the cache up to date
        cacheHandler.InvalidateCacheById(updateBookingDtoRequest.Id);
        cacheHandler.InvalidateCacheByListType(CacheListType.Booking);

        return updateBookingDtoResponse;
    }

    public async Task<DeleteBookingDtoResponse> DeleteAsync(DeleteBookingDtoRequest deleteBookingDtoRequest)
    {
        var deleteBookingDtoResponse = new DeleteBookingDtoResponse();
        var validator = new DeleteBookingValidator();
        var result = validator.Validate(deleteBookingDtoRequest);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        _ = await bookingRepository.GetByIdAsync(deleteBookingDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.BookingNotFound);
        await bookingRepository.DeleteAsync(deleteBookingDtoRequest.Id);

        // Keep cache up to date
        cacheHandler.InvalidateCacheById(deleteBookingDtoRequest.Id);
        cacheHandler.InvalidateCacheByListType(CacheListType.Booking);

        return deleteBookingDtoResponse;
    }

    public async Task<GetBookingDtoResponse> GetByIdAsync(GetBookingDtoRequest getBookingDtoRequest)
    {
        var getBookingDtoResponse = new GetBookingDtoResponse();

        var validator = new GetBookingValidator();
        var result = validator.Validate(getBookingDtoRequest);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        return await cacheHandler.GetOrCreateRecordAsync(
            getBookingDtoRequest,
            async () =>
            {
                getBookingDtoResponse.Booking = await bookingRepository.GetByIdAsync(getBookingDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.BookingNotFound);
                return getBookingDtoResponse;
            },
            new CacheOptions
            {
                Id = getBookingDtoRequest.Id,
                AbsoluteExpireTime = TimeSpan.FromDays(1)
            }
        );
    }

    public async Task<PaginatedBookingDtoResponse> GetFilteredAsync(PaginatedBookingDtoRequest paginatedBookingDtoRequest)
    {
        var paginatedBookingDtoResponse = new PaginatedBookingDtoResponse();

        var validator = new PaginatedBookingValidator();
        var result = validator.Validate(paginatedBookingDtoRequest);
        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        return await cacheHandler.GetOrCreateRecordAsync(
            paginatedBookingDtoRequest,
            async () =>
            {
                var bookingDtoList = await bookingRepository.GetFilteredAsync(paginatedBookingDtoRequest.Filter);
                if (bookingDtoList.Count == 0)
                    throw new KeyNotFoundException(Messages.BookingNotFound);

                paginatedBookingDtoResponse.TotalItems = bookingDtoList.Count();
                var nrPages = decimal.Divide(paginatedBookingDtoResponse.TotalItems, paginatedBookingDtoRequest.Filter.PageSize);
                paginatedBookingDtoResponse.TotalPages = (int)(nrPages - Math.Truncate(nrPages) > 0 ? Math.Truncate(nrPages) + 1 : Math.Truncate(nrPages));
                bookingDtoList = bookingDtoList.OrderBy(x => x.Id).ToList();
                bookingDtoList = bookingDtoList.Skip((paginatedBookingDtoRequest.Filter.PageNumber - 1) * paginatedBookingDtoRequest.Filter.PageSize).Take(paginatedBookingDtoRequest.Filter.PageSize).ToList();

                paginatedBookingDtoResponse.Bookings = bookingDtoList;
                return paginatedBookingDtoResponse;
            },
            new CacheOptions
            {
                ListType = CacheListType.Booking,
                AbsoluteExpireTime = TimeSpan.FromMinutes(5)
            }
        );
    }
}