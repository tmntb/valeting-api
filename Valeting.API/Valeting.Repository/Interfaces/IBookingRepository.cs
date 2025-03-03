using Valeting.Common.Models.Booking;

namespace Valeting.Repository.Interfaces;

public interface IBookingRepository
{
    Task CreateAsync(BookingDto bookingDto);
    Task UpdateAsync(BookingDto bookingDto);
    Task DeleteAsync(Guid id);
    Task<BookingListDto> GetAsync(BookingFilterDto bookingFilterDto);
    Task<BookingDto> GetByIdAsync(Guid id);
}