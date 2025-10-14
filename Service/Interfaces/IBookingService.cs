using Common.Models.Booking;

namespace Service.Interfaces;

public interface IBookingService
{
    Task<Guid> CreateAsync(BookingDto bookingDto);
    Task UpdateAsync(BookingDto bookingDto);
    Task DeleteAsync(Guid id);
    Task<BookingDto> GetByIdAsync(Guid id);
    Task<BookingPaginatedDtoResponse> GetFilteredAsync(BookingFilterDto bookingFilterDto);
}