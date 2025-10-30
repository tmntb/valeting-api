using Api.Models.Flexibility;
using Api.Models.VehicleSize;

namespace Api.Models.Booking.Payload;

/// <summary>
/// Represents the request body for updating an existing booking.
/// </summary>
public class UpdateBookingApiRequest
{
    /// <summary>
    /// The name of the booking.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The date and time of the booking.
    /// </summary>
    public DateTime BookingDate { get; set; }

    /// <summary>
    /// The flexibility information associated with the booking.
    /// </summary>
    public FlexibilityApi Flexibility { get; set; }

    /// <summary>
    /// The vehicle size information associated with the booking.
    /// </summary>
    public VehicleSizeApi VehicleSize { get; set; }

    /// <summary>
    /// The contact number for the booking.
    /// </summary>
    public int? ContactNumber { get; set; }

    /// <summary>
    /// The email address associated with the booking.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Indicates whether the booking has been approved.
    /// </summary>
    public bool? Approved { get; set; }
}
