using Service.Models.Flexibility;
using Service.Models.VehicleSize;

namespace Service.Models.Booking;

/// <summary>
/// Represents a booking with all its associated details including customer info, vehicle size, and flexibility options.
/// </summary>
public class BookingDto
{
    /// <summary>
    /// The unique identifier of the booking.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the person who made the booking.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The date and time of the booking.
    /// </summary>
    public DateTime BookingDate { get; set; }

    /// <summary>
    /// The flexibility option associated with the booking.
    /// </summary>
    public FlexibilityDto Flexibility { get; set; } = new();

    /// <summary>
    /// The vehicle size associated with the booking.
    /// </summary>
    public VehicleSizeDto VehicleSize { get; set; } = new();

    /// <summary>
    /// Indicates whether the booking has been approved.
    /// </summary>
    public bool? Approved { get; set; }
}
