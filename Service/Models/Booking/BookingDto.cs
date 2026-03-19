using Service.Models.Flexibility;
using Service.Models.Status;
using Service.Models.User;
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
    /// The reference of the booking.
    /// </summary>
    public string Reference { get; set; }

    /// <summary>
    /// The customer associated with the booking.
    /// </summary>
    public UserDto Customer { get; set; }

    /// <summary>
    /// The flexibility option associated with the booking.
    /// </summary>
    public FlexibilityDto Flexibility { get; set; }

    /// <summary>
    /// The vehicle size associated with the booking.
    /// </summary>
    public VehicleSizeDto VehicleSize { get; set; }

    /// <summary>
    /// The date and time of the booking.
    /// </summary>
    public DateTime ScheduledAt { get; set; }

    /// <summary>
    /// The status of the booking.
    /// </summary>
    public StatusDto Status { get; set; }

    /// <summary>
    /// The date and time when the booking was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time when the booking was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// The date and time when the booking decision was made.
    /// </summary>
    public DateTime? DecisionAt { get; set; }

    /// <summary>
    /// The user who made the decision on the booking.
    /// </summary>
    public UserDto? Decision { get; set; }

    /// <summary>
    /// Indicates whether the booking has been approved.
    /// </summary>
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// Any additional notes for the booking.
    /// </summary>
    public string Notes { get; set; }
}
