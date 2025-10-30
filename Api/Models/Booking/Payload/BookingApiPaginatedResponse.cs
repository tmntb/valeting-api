using Api.Models.Core;

namespace Api.Models.Booking.Payload;

/// <summary>
/// Represents a paginated response containing a list of bookings.
/// </summary>
public class BookingApiPaginatedResponse : PaginationApi
{
    /// <summary>
    /// The list of bookings for the current page.
    /// </summary>
    public List<BookingApi> Bookings { get; set; }
}
