namespace Api.Models.Booking.Payload;

/// <summary>
/// Represents a response containing a single booking.
/// </summary>
public class BookingApiResponse
{
    /// <summary>
    /// The booking included in the response.
    /// </summary>
    public BookingApi Booking { get; set; }
}

