using AutoMapper;
using FluentValidation;
using Valeting.Common.Cache;
using Valeting.Common.Cache.Interfaces;
using Valeting.Common.Messages;
using Valeting.Common.Models.Booking;
using Valeting.Core.Interfaces;
using Valeting.Core.Validators;
using Valeting.Core.Validators.Utils;
using Valeting.Repository.Interfaces;

namespace Valeting.Core.Services;

public class BookingService(IBookingRepository bookingRepository, ICacheHandler cacheHandler, IMapper mapper) : IBookingService
{
    public async Task<CreateBookingDtoResponse> CreateAsync(CreateBookingDtoRequest createBookingDtoRequest)
    {
        var createBookingDtoResponse = new CreateBookingDtoResponse();

        createBookingDtoRequest.ValidateRequest(new CreateBookingValidator());

        var id = Guid.NewGuid();
        var bookingDto = mapper.Map<BookingDto>(createBookingDtoRequest);
        bookingDto.Id = id;

        await bookingRepository.CreateAsync(bookingDto);
        createBookingDtoResponse.Id = id;

        cacheHandler.InvalidateCacheByListType(CacheListType.Booking);

        return createBookingDtoResponse;
    }

    public async Task UpdateAsync(UpdateBookingDtoRequest updateBookingDtoRequest)
    {
        updateBookingDtoRequest.ValidateRequest(new UpdateBookinValidator());

        var bookingDto = await bookingRepository.GetByIdAsync(updateBookingDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.NotFound);

        mapper.Map(updateBookingDtoRequest, bookingDto);
        await bookingRepository.UpdateAsync(bookingDto);

        // Keep the cache up to date
        cacheHandler.InvalidateCacheById(updateBookingDtoRequest.Id);
        cacheHandler.InvalidateCacheByListType(CacheListType.Booking);
    }

    public async Task DeleteAsync(DeleteBookingDtoRequest deleteBookingDtoRequest)
    {
        deleteBookingDtoRequest.ValidateRequest(new DeleteBookingValidator());

        _ = await bookingRepository.GetByIdAsync(deleteBookingDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.NotFound);
        await bookingRepository.DeleteAsync(deleteBookingDtoRequest.Id);

        // Keep cache up to date
        cacheHandler.InvalidateCacheById(deleteBookingDtoRequest.Id);
        cacheHandler.InvalidateCacheByListType(CacheListType.Booking);
    }

    public async Task<GetBookingDtoResponse> GetByIdAsync(GetBookingDtoRequest getBookingDtoRequest)
    {
        var getBookingDtoResponse = new GetBookingDtoResponse();

        getBookingDtoRequest.ValidateRequest(new GetBookingValidator());

        return await cacheHandler.GetOrCreateRecordAsync(
            getBookingDtoRequest,
            async () =>
            {
                getBookingDtoResponse.Booking = await bookingRepository.GetByIdAsync(getBookingDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.NotFound);
                return getBookingDtoResponse;
            },
            new()
            {
                Id = getBookingDtoRequest.Id,
                AbsoluteExpireTime = TimeSpan.FromDays(1)
            }
        );
    }

    public async Task<PaginatedBookingDtoResponse> GetFilteredAsync(PaginatedBookingDtoRequest paginatedBookingDtoRequest)
    {
        var paginatedBookingDtoResponse = new PaginatedBookingDtoResponse();

        paginatedBookingDtoRequest.ValidateRequest(new PaginatedBookingValidator());

        return await cacheHandler.GetOrCreateRecordAsync(
            paginatedBookingDtoRequest,
            async () =>
            {
                var bookingDtoList = await bookingRepository.GetFilteredAsync(paginatedBookingDtoRequest.Filter);
                if (bookingDtoList.Count == 0)
                    throw new KeyNotFoundException(Messages.NotFound);

                paginatedBookingDtoResponse.TotalItems = bookingDtoList.Count();
                paginatedBookingDtoResponse.TotalPages = (int)Math.Ceiling((double)paginatedBookingDtoResponse.TotalItems / paginatedBookingDtoRequest.Filter.PageSize);

                bookingDtoList = bookingDtoList
                    .OrderBy(x => x.Id)
                    .Skip((paginatedBookingDtoRequest.Filter.PageNumber - 1) * paginatedBookingDtoRequest.Filter.PageSize)
                    .Take(paginatedBookingDtoRequest.Filter.PageSize)
                    .ToList();

                paginatedBookingDtoResponse.Bookings = bookingDtoList;
                return paginatedBookingDtoResponse;
            },
            new()
            {
                ListType = CacheListType.Booking,
                AbsoluteExpireTime = TimeSpan.FromMinutes(5)
            }
        );
    }
}