using Valeting.Business.Booking;
using Valeting.Services.Objects.Booking;

namespace Valeting.Services.Interfaces;

public interface IBookingService
{
    Task<CreateBookingSVResponse> CreateAsync(CreateBookingSVRequest createBookingSVRequest);
    Task UpdateAsync(BookingDTO bookingDTO);
    Task DeleteAsync(Guid id);
    Task<BookingDTO> FindByIDAsync(Guid id);
    Task<BookingListDTO> ListAllAsync(BookingFilterDTO bookingFilterDTO);
}