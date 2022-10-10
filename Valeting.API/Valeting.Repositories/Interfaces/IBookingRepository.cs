using Valeting.Business;

namespace Valeting.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task CreateAsync(BookingDTO bookingDTO);
        Task UpdateAsync(BookingDTO bookingDTO);
        Task DeleteAsync(Guid id);
        Task<BookingDTO> FindByIDAsync(Guid id);
        Task<IEnumerable<BookingDTO>> ListAsync();
    }
}
