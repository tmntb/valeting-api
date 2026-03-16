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

        bookingDto.Status = statusDto;
        bookingDto.UpdatedAt = DateTime.Now;

        if (updateBookingStatusDtoRequest.Status == StatusEnum.APPROVED || updateBookingStatusDtoRequest.Status == StatusEnum.REJECTED || updateBookingStatusDtoRequest.Status == StatusEnum.CANCELLED)
        {
            bookingDto.DecisionAt = DateTime.Now;
            bookingDto.Decision = updateBookingStatusDtoRequest.UserDto;
            bookingDto.RequiresApproval = false;
        }

        await bookingRepository.UpdateAsync(bookingDto);
    }

    /// <inheritdoc />
    public async Task<BookingDto> GetByIdAsync(Guid id)
    {
        return await bookingRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException(Messages.NotFound);
    }

    /// <inheritdoc />
    public async Task<BookingPaginatedDtoResponse> GetCustomerFilteredAsync(BookingFilterDto bookingFilterDto)
    {
        bookingFilterDto.ValidateRequest(new PaginatedBookingCustomerValidator());

        var bookingDtoList = await bookingRepository.GetFilteredAsync(bookingFilterDto);
        return CreatePaginatedResponse(bookingDtoList, bookingFilterDto);
    }

    /// <inheritdoc />
    public async Task<BookingPaginatedDtoResponse> GetFilteredAsync(BookingFilterDto bookingFilterDto)
    {
        bookingFilterDto.ValidateRequest(new PaginatedBookingValidator());

        var bookingDtoList = await bookingRepository.GetFilteredAsync(bookingFilterDto);
        return CreatePaginatedResponse(bookingDtoList, bookingFilterDto);
    }

    /// <summary>
    /// Creates a paginated response for a list of bookings based on the provided filter criteria.
    /// </summary>
    /// <param name="bookingDtoList">The list of <see cref="BookingDto"/> to paginate.</param>
    /// <param name="bookingFilterDto">The filter criteria used for pagination, including page number and page size.</param>
    /// <returns>A <see cref="BookingPaginatedDtoResponse"/> containing the paginated list of bookings and pagination metadata.</returns>
    private static BookingPaginatedDtoResponse CreatePaginatedResponse(List<BookingDto> bookingDtoList, BookingFilterDto bookingFilterDto)
    {
        if (bookingDtoList.Count == 0)
            throw new KeyNotFoundException(Messages.NotFound);

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
}