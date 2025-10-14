namespace Common.Models.Booking;

public class GetBookingDtoRequest
{
    public Guid Id { get; set; }
}

public class GetBookingDtoResponse
{
    public BookingDto Booking { get; set; }
}

public class PaginatedBookingDtoRequest
{
    public BookingFilterDto Filter { get; set; }
}

public class BookingPaginatedDtoResponse
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<BookingDto> Bookings { get; set; }
}