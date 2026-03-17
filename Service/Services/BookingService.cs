using Common.Enums;
using Common.Messages;
using Service.Interfaces;
using Service.Models.Booking;
using Service.Models.Booking.Payload;
using Service.Validators;
using Service.Validators.Utils;

namespace Service.Services;

public class BookingService(IBookingRepository bookingRepository, IStatusRepository statusRepository) : IBookingService
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
    }

    /// <inheritdoc />
    public async Task UpdateStatusAsync(UpdateBookingStatusDtoRequest updateBookingStatusDtoRequest)
    {
        var bookingDto = await bookingRepository.GetByIdAsync(updateBookingStatusDtoRequest.Id) ?? throw new KeyNotFoundException(Messages.NotFound);
        updateBookingStatusDtoRequest.CurrentStatus = bookingDto.Status.Code;

        var statusDto = await statusRepository.GetByCodeAsync(updateBookingStatusDtoRequest.Status) ?? throw new KeyNotFoundException(Messages.NotFound);

        updateBookingStatusDtoRequest.ValidateRequest(new UpdateBookingStatusValidator());

        var now = DateTime.UtcNow;

        bookingDto.Status = statusDto;
        bookingDto.UpdatedAt = now;
        bookingDto.RequiresApproval = false;

        if (updateBookingStatusDtoRequest.Status == StatusEnum.APPROVED || updateBookingStatusDtoRequest.Status == StatusEnum.REJECTED)
        {
            bookingDto.DecisionAt = now;
            bookingDto.Decision = updateBookingStatusDtoRequest.UserDto;
        }

        await bookingRepository.UpdateAsync(bookingDto);
    }

    /// <inheritdoc />
    public async Task<BookingDto> GetByIdAsync(Guid id)
    {
        var bookingDto = await bookingRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException(Messages.NotFound);
        return await CheckStatusAsync(bookingDto);
    }

    /// <inheritdoc />
    public async Task<BookingPaginatedDtoResponse> GetCustomerFilteredAsync(BookingFilterDto bookingFilterDto)
    {
        bookingFilterDto.ValidateRequest(new PaginatedBookingCustomerValidator());

        return await CreatePaginatedResponseAsync(bookingFilterDto);
    }

    /// <inheritdoc />
    public async Task<BookingPaginatedDtoResponse> GetFilteredAsync(BookingFilterDto bookingFilterDto)
    {
        bookingFilterDto.ValidateRequest(new PaginatedBookingValidator());

        return await CreatePaginatedResponseAsync(bookingFilterDto);
    }

    private async Task<BookingPaginatedDtoResponse> CreatePaginatedResponseAsync(BookingFilterDto bookingFilterDto)
    {
        var bookingDtoList = await bookingRepository.GetFilteredAsync(bookingFilterDto);

        if (bookingDtoList.Count == 0)
            throw new KeyNotFoundException(Messages.NotFound);

        // Ensure status updates run within the request scope and are awaited.
        await Task.WhenAll(bookingDtoList.Select(CheckStatusAsync));

        var totalItems = bookingDtoList.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / bookingFilterDto.PageSize);

        var pagedBookings = bookingDtoList
            .OrderBy(x => x.Id)
            .Skip((bookingFilterDto.PageNumber - 1) * bookingFilterDto.PageSize)
            .Take(bookingFilterDto.PageSize)
            .ToList();

        return new()
        {
            TotalItems = totalItems,
            TotalPages = totalPages,
            Bookings = pagedBookings
        };
    }

    /// <summary>
    /// Checks the status of a booking and updates it if necessary based on the current time and the booking's scheduled time and flexibility.
    /// If the booking has passed its end time and is still in APPROVED status, it will be updated to COMPLETED. If the booking has passed its scheduled time and is still in PENDING_APPROVAL status, it will be updated to EXPIRED.
    /// </summary>
    /// <param name="bookingDto">The booking DTO to check and potentially update.</param>
    /// <returns>The updated booking DTO after checking and potentially updating its status.</returns>
    private async Task<BookingDto> CheckStatusAsync(BookingDto bookingDto)
    {
        var now = DateTime.UtcNow;
        var endBookingTime = bookingDto.ScheduledAt.AddMinutes(bookingDto.Flexibility.NumberOfMinutes);
        if (endBookingTime < now && bookingDto.Status.Code == StatusEnum.APPROVED)
        {
            var statusDto = await statusRepository.GetByCodeAsync(StatusEnum.COMPLETED) ?? throw new KeyNotFoundException(Messages.NotFound);
            bookingDto.Status = statusDto;
            bookingDto.UpdatedAt = now;
            bookingDto.RequiresApproval = false;

            await bookingRepository.UpdateAsync(bookingDto);
        }

        if (bookingDto.ScheduledAt < now && bookingDto.Status.Code == StatusEnum.PENDING_APPROVAL)
        {
            var statusDto = await statusRepository.GetByCodeAsync(StatusEnum.EXPIRED) ?? throw new KeyNotFoundException(Messages.NotFound);
            bookingDto.Status = statusDto;
            bookingDto.UpdatedAt = now;
            bookingDto.RequiresApproval = false;

            await bookingRepository.UpdateAsync(bookingDto);
        }

        return bookingDto;
    }
}