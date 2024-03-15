using Valeting.Business.Booking;

namespace Valeting.Services.Interfaces;

public interface IBookingService
{
    Task<BookingDTO> CreateAsync(BookingDTO bookingDTO);
    Task UpdateAsync(BookingDTO bookingDTO);
    Task DeleteAsync(Guid id);
    Task<BookingDTO> FindByIDAsync(Guid id);
    Task<BookingListDTO> ListAllAsync(BookingFilterDTO bookingFilterDTO);
}