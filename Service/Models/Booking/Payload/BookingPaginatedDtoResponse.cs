namespace Service.Models.Booking.Payload;

public class BookingPaginatedDtoResponse
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<BookingDto> Bookings { get; set; }
}