using Valeting.Business.Core;

namespace Valeting.Business.Booking;

public class BookingListDTO : ContentDTO
{
    public List<BookingDTO> Bookings { get; set; }
}