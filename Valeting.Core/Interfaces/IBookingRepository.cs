using Valeting.Common.Models.Booking;

namespace Valeting.Core.Interfaces;

public interface IBookingRepository
{
    Task CreateAsync(BookingDto bookingDto);
    Task UpdateAsync(BookingDto bookingDto);
    Task DeleteAsync(Guid id);
    Task<List<BookingDto>> GetFilteredAsync(BookingFilterDto bookingFilterDto);
    Task<BookingDto> GetByIdAsync(Guid id);
}