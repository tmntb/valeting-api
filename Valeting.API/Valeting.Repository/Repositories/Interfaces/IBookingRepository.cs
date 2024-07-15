using Valeting.Repository.Models.Booking;

namespace Valeting.Repository.Repositories.Interfaces;

public interface IBookingRepository
{
    Task CreateAsync(BookingDTO bookingDTO);
    Task UpdateAsync(BookingDTO bookingDTO);
    Task DeleteAsync(Guid id);
    Task<BookingDTO> GetByIDAsync(Guid id);
    Task<BookingListDTO> GetAsync(BookingFilterDTO bookingFilterDTO);
}