using Valeting.Business;

namespace Valeting.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDTO> CreateAsync(BookingDTO bookingDTO);
        Task UpdateAsync(BookingDTO bookingDTO);
        Task DeleteAsync(Guid id);
        Task<BookingDTO> FindByIDAsync(Guid id);
        Task<IEnumerable<BookingDTO>> ListAllAsync();
    }
}

