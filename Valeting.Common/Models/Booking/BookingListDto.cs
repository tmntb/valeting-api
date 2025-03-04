using Valeting.Common.Models.Core;

namespace Valeting.Common.Models.Booking;

public class BookingListDto : ContentDto
{
    public List<BookingDto> Bookings { get; set; }
}