namespace Api.Models.Booking.Payload;

/// <summary>
/// Represents the request body for creating a new booking.
/// </summary>
public class CreateBookingApiRequest
{
    /// <summary>
    /// The flexibility options associated with the booking.
    /// </summary>
    public Guid FlexibilityId { get; set; }

    /// <summary>
    /// The vehicle size associated with the booking.
    /// </summary>
    public Guid VehicleSizeId { get; set; }

    /// <summary>
    /// The date and time of the booking.
    /// </summary>
    public DateTime ScheduledAt { get; set; }

    /// <summary>
    /// Additional notes for the booking.
    /// </summary>
    public string? Notes { get; set; }
}
