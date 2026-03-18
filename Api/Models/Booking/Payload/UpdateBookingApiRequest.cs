namespace Api.Models.Booking.Payload;

/// <summary>
/// Represents the request body for updating an existing booking.
/// </summary>
public class UpdateBookingApiRequest
{
    /// <summary>
    /// The flexibility information associated with the booking.
    /// </summary>
    public Guid FlexibilityId { get; set; }

    /// <summary>
    /// The vehicle size information associated with the booking.
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
