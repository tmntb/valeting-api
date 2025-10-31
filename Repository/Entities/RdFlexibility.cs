namespace Repository.Entities;

/// <summary>
/// Represents a flexibility option that can be associated with bookings.
/// </summary>
public partial class RdFlexibility
{
    /// <summary>
    /// Initializes a new instance of <see cref="RdFlexibility"/> and its bookings collection.
    /// </summary>
    public RdFlexibility()
    {
        Bookings = new HashSet<Booking>();
    }

    /// <summary>
    /// Unique identifier for the flexibility option.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Description of the flexibility option.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Indicates whether the flexibility option is active.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Collection of bookings associated with this flexibility option.
    /// </summary>
    public virtual ICollection<Booking> Bookings { get; set; }
}
