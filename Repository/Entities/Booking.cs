namespace Repository.Entities;

/// <summary>
/// Represents a booking made by a customer, including references to flexibility and vehicle size options.
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
    public string Name { get; set; } = null!;

    /// <summary>
    /// Date and time of the booking.
    /// </summary>
    public DateTime BookingDate { get; set; }

    /// <summary>
    /// Foreign key referencing the selected flexibility option.
    /// </summary>
    public Guid FlexibilityId { get; set; }

    /// <summary>
    /// Foreign key referencing the selected vehicle size option.
    /// </summary>
    public Guid VehicleSizeId { get; set; }

    /// <summary>
    /// Indicates whether the booking has been approved.
    /// </summary>
    public bool? Approved { get; set; }

    /// <summary>
    /// Navigation property for the selected flexibility option.
    /// </summary>
    public virtual RdFlexibility Flexibility { get; set; } = null!;

    /// <summary>
    /// Navigation property for the selected vehicle size option.
    /// </summary>
    public virtual RdVehicleSize VehicleSize { get; set; } = null!;
}
