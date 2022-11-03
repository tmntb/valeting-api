using Valeting.Business.Booking;

namespace Valeting.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task CreateAsync(BookingDTO bookingDTO);
        Task UpdateAsync(BookingDTO bookingDTO);
        Task DeleteAsync(Guid id);
        Task<BookingDTO> FindByIdAsync(Guid id);
        Task<BookingListDTO> ListAsync(BookingFilterDTO bookingFilterDTO);
    }
}
