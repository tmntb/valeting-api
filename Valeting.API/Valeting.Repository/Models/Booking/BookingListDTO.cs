using Valeting.Repository.Models.Core;

namespace Valeting.Repository.Models.Booking;

public class BookingListDTO : ContentDTO
{
    public List<BookingDTO> Bookings { get; set; }
}