using Common.Models.Booking;

namespace Service.Interfaces;

public interface IBookingRepository
{
    Task CreateAsync(BookingDto bookingDto);
    Task UpdateAsync(BookingDto bookingDto);
    Task DeleteAsync(Guid id);
    Task<List<BookingDto>> GetFilteredAsync(BookingFilterDto bookingFilterDto);
    Task<BookingDto> GetByIdAsync(Guid id);
}