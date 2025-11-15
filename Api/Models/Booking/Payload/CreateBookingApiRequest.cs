using Api.Models.Flexibility;
using Api.Models.VehicleSize;

namespace Api.Models.Booking.Payload;

/// <summary>
/// Represents the request body for creating a new booking.
/// </summary>
public class CreateBookingApiRequest
{
    /// <summary>
    /// The name of the person making the booking.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The date and time of the booking.
    /// </summary>
    public DateTime BookingDate { get; set; }

    /// <summary>
    /// The flexibility options associated with the booking.
    /// </summary>
    public FlexibilityApi Flexibility { get; set; }

    /// <summary>
    /// The vehicle size associated with the booking.
    /// </summary>
    public VehicleSizeApi VehicleSize { get; set; }

    /// <summary>
    /// The contact number of the person making the booking (optional).
    /// </summary>
    public int? ContactNumber { get; set; }

    /// <summary>
    /// The email address of the person making the booking.
    /// </summary>
    public string Email { get; set; }
}
