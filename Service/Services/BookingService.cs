using Common.Cache;
using Common.Cache.Interfaces;
using Common.Enums;
using Common.Messages;
using Service.Interfaces;
using Service.Models.Booking;
using Service.Models.Booking.Payload;
using Service.Validators;
using Service.Validators.Utils;

namespace Service.Services;

public class BookingService(IBookingRepository bookingRepository, IStatusRepository statusRepository, ICacheHandler cacheHandler) : IBookingService
{
    /// <inheritdoc />
    public async Task<Guid> CreateAsync(BookingDto bookingDto)
    {
        bookingDto.ValidateRequest(new CreateBookingValidator());

        var id = Guid.NewGuid();
        var shortId = id.ToString("N")[..6].ToUpper();
        var statusDto = await statusRepository.GetByCodeAsync(StatusEnum.PENDING_APPROVAL) ?? throw new KeyNotFoundException(Messages.NotFound);

        bookingDto.Id = id;
        bookingDto.Reference = $"BK-{DateTime.UtcNow:yyyyMMdd}-{shortId}";
        bookingDto.Status = statusDto;
        bookingDto.CreatedAt = DateTime.UtcNow;
        bookingDto.RequiresApproval = true;

        await bookingRepository.CreateAsync(bookingDto);

        cacheHandler.InvalidateCacheByListType(CacheListType.Booking);

        return id;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(BookingDto bookingDto)
    {
        bookingDto.ValidateRequest(new UpdateBookingValidator());

        var bookingDtoToUpdate = await bookingRepository.GetByIdAsync(bookingDto.Id) ?? throw new KeyNotFoundException(Messages.NotFound);
        if (!Equals(bookingDtoToUpdate.Status.Code, StatusEnum.PENDING_APPROVAL))
            throw new InvalidOperationException(Messages.InvalidBookingStatusForUpdate);

        bookingDtoToUpdate.Flexibility = bookingDto.Flexibility;
        bookingDtoToUpdate.VehicleSize = bookingDto.VehicleSize;
        bookingDtoToUpdate.ScheduledAt = bookingDto.ScheduledAt;
        bookingDtoToUpdate.UpdatedAt = DateTime.UtcNow;
        bookingDtoToUpdate.RequiresApproval = true;
        bookingDtoToUpdate.Notes = bookingDto.Notes;

        await bookingRepository.UpdateAsync(bookingDtoToUpdate);

        // Keep the cache up to date
        cacheHandler.InvalidateCacheById(bookingDto.Id);
        cacheHandler.InvalidateCacheByListType(CacheListType.Booking);
    }

    /// <inheritdoc />
    public async Task UpdateStatusAsync(UpdateBookingStatusDtoRequest updateBookingStatusDtoRequest)
    {
        var bookingDto = await bookingRepository.GetByIdAsync(updateBookingStatusDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.NotFound);
        updateBookingStatusDtoRequest.CurrentStatus = bookingDto.Status.Code;

        var statusDto = await statusRepository.GetByCodeAsync(updateBookingStatusDtoRequest.Status) ?? throw new KeyNotFoundException(Messages.NotFound);

        updateBookingStatusDtoRequest.ValidateRequest(new UpdateBookingStatusValidator());

        bookingDto.Status = statusDto;
        bookingDto.UpdatedAt = DateTime.Now;

        if (updateBookingStatusDtoRequest.Status == StatusEnum.APPROVED || updateBookingStatusDtoRequest.Status == StatusEnum.REJECTED || updateBookingStatusDtoRequest.Status == StatusEnum.CANCELLED)
        {
            bookingDto.DecisionAt = DateTime.Now;
            bookingDto.Decision = updateBookingStatusDtoRequest.UserDto;
            bookingDto.RequiresApproval = false;
        }

        await bookingRepository.UpdateAsync(bookingDto);

        // Keep the cache up to date
        cacheHandler.InvalidateCacheById(bookingDto.Id);
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