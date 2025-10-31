namespace Service.Models.Booking.Payload;

/// <summary>
/// Represents a paginated response containing a list of bookings.
/// </summary>
public class BookingPaginatedDtoResponse
{
    /// <summary>
    /// Total number of bookings available.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total number of pages based on the requested page size.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// The list of bookings for the current page.
    /// </summary>
    public List<BookingDto> Bookings { get; set; }
}
