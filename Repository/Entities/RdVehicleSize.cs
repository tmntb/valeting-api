namespace Repository.Entities;

/// <summary>
/// Represents a vehicle size option that can be associated with bookings.
/// </summary>
public partial class RdVehicleSize
{
    /// <summary>
    /// Initializes a new instance of <see cref="RdVehicleSize"/> and its bookings collection.
    /// </summary>
    public RdVehicleSize()
    {
        Bookings = new HashSet<Booking>();
    }

    /// <summary>
    /// Unique identifier for the vehicle size.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Description of the vehicle size.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Indicates whether the vehicle size is active.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Collection of bookings associated with this vehicle size.
    /// </summary>
    public virtual ICollection<Booking> Bookings { get; set; }
}
