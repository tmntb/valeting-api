namespace Api.Models.Booking.Payload;

/// <summary>
/// Represents the response returned after successfully creating a booking.
/// </summary>
public class CreateBookingApiResponse
{
    /// <summary>
    /// The unique identifier of the newly created booking.
    /// </summary>
    public Guid Id { get; set; }
}
