using Common.Cache;
using Common.Cache.Interfaces;
using Common.Messages;
using Service.Interfaces;
using Service.Models.Booking;
using Service.Models.Booking.Payload;
using Service.Validators;
using Service.Validators.Utils;

namespace Service.Services;

public class BookingService(IBookingRepository bookingRepository, ICacheHandler cacheHandler) : IBookingService
{
    /// <inheritdoc />
    public async Task<Guid> CreateAsync(BookingDto bookingDto)
    {
        bookingDto.ValidateRequest(new CreateBookingValidator());

        var id = Guid.NewGuid();
        bookingDto.Id = id;

        await bookingRepository.CreateAsync(bookingDto);

        cacheHandler.InvalidateCacheByListType(CacheListType.Booking);

        return id;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(BookingDto bookingDto)
    {
        bookingDto.ValidateRequest(new UpdateBookingValidator());

        var bookingDtoToUpdate = await bookingRepository.GetByIdAsync(bookingDto.Id) ?? throw new KeyNotFoundException(Messages.NotFound);
        bookingDtoToUpdate.Name = bookingDto.Name;
        bookingDtoToUpdate.BookingDate = bookingDto.BookingDate;
        bookingDtoToUpdate.Approved = bookingDto.Approved;
        bookingDtoToUpdate.Flexibility = bookingDto.Flexibility;
        bookingDtoToUpdate.VehicleSize = bookingDto.VehicleSize;

        await bookingRepository.UpdateAsync(bookingDto);

        // Keep the cache up to date
        cacheHandler.InvalidateCacheById(bookingDto.Id);
        cacheHandler.InvalidateCacheByListType(CacheListType.Booking);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        _ = await bookingRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException(Messages.NotFound);
        await bookingRepository.DeleteAsync(id);

        // Keep cache up to date
        cacheHandler.InvalidateCacheById(id);
        cacheHandler.InvalidateCacheByListType(CacheListType.Booking);
    }

    /// <inheritdoc />
    public async Task<BookingDto> GetByIdAsync(Guid id)
    {
        return await cacheHandler.GetOrCreateRecordAsync(
            id,
            async () =>
            {
                return await bookingRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException(Messages.NotFound);
            },
            new()
            {
                Id = id,
                AbsoluteExpireTime = TimeSpan.FromDays(1)
            }
        );
    }

    /// <inheritdoc />
    public async Task<BookingPaginatedDtoResponse> GetFilteredAsync(BookingFilterDto bookingFilterDto)
    {
        var paginatedBookingDtoResponse = new BookingPaginatedDtoResponse();

        bookingFilterDto.ValidateRequest(new PaginatedBookingValidator());

        return await cacheHandler.GetOrCreateRecordAsync(
            bookingFilterDto,
            async () =>
            {
                var bookingDtoList = await bookingRepository.GetFilteredAsync(bookingFilterDto);
                if (bookingDtoList.Count == 0)
                    throw new KeyNotFoundException(Messages.NotFound);

                paginatedBookingDtoResponse.TotalItems = bookingDtoList.Count();
                paginatedBookingDtoResponse.TotalPages = (int)Math.Ceiling((double)paginatedBookingDtoResponse.TotalItems / bookingFilterDto.PageSize);

                bookingDtoList = bookingDtoList
                    .OrderBy(x => x.Id)
                    .Skip((bookingFilterDto.PageNumber - 1) * bookingFilterDto.PageSize)
                    .Take(bookingFilterDto.PageSize)
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