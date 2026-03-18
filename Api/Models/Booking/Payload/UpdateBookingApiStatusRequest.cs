using Common.Enums;

namespace Api.Models.Booking.Payload;

/// <summary>
/// Represents the request body for updating the status of a booking.
/// </summary>
public class UpdateBookingApiStatusRequest
{
    /// <summary>
    /// The new status to set for the booking.
    /// </summary>
    public StatusEnum Status { get; set; }
}
