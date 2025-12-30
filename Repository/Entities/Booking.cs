namespace Repository.Entities;

/// <summary>
/// Represents a booking made by a customer.
/// </summary>
public partial class Booking
{
    /// <summary>
    /// Unique identifier for the booking.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the person who made the booking.
    /// </summary>
    public string Reference { get; set; }

    /// <summary>
    /// Foreign key referencing the customer who made the booking.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Foreign key referencing the selected flexibility option.
    /// </summary>
    public Guid FlexibilityId { get; set; }

    /// <summary>
    /// Foreign key referencing the selected vehicle size option.
    /// </summary>
    public Guid VehicleSizeId { get; set; }

    /// <summary>
    /// Date and time of the booking.
    /// </summary>
    public DateTime ScheduledAt { get; set; }

    /// <summary>
    /// Foreign key referencing the current status of the booking.
    /// </summary>
    public Guid StatusId { get; set; }

    /// <summary>
    /// Timestamp when the booking was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the booking was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicates whether the booking has been decided.
    /// </summary>
    public DateTime? DecisionAt { get; set; }

    /// <summary>
    /// Foreign key referencing the user who decided the booking.
    /// </summary>
    public Guid? DecisionById { get; set; }

    /// <summary>
    /// Indicates whether the booking requires approval.
    /// </summary>
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// Additional notes or comments related to the booking.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property for the customer who made the booking.
    /// </summary>
    public virtual ApplicationUser Customer { get; set; } = null!;

    /// <summary>
    /// Navigation property for the selected flexibility option.
    /// </summary>
    public virtual RdFlexibility Flexibility { get; set; } = null!;

    /// <summary>
    /// Navigation property for the selected vehicle size option.
    /// </summary>
    public virtual RdVehicleSize VehicleSize { get; set; } = null!;

    /// <summary>
    /// Navigation property for the current status of the booking.
    /// </summary>
    public virtual RdStatus Status { get; set; } = null!;

    /// <summary>
    /// Navigation property for the user who decided the booking.
    /// </summary>
    public virtual ApplicationUser? DecisionBy { get; set; }
}
