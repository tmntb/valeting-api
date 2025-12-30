using Common.Enums;

namespace Repository.Entities;

/// <summary>
/// Represents a status that can be assigned to bookings.
/// </summary>
public partial class RdStatus
{
    /// <summary>
    /// Initializes a new instance of <see cref="RdStatus"/> and its bookings collection.
    /// </summary>
    public RdStatus()
    {
        Bookings = new HashSet<Booking>();
    }

    /// <summary>
    /// Unique identifier for the status.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Code representing the status.
    /// </summary>
    public StatusCodeEnum Code { get; set; }

    /// <summary>
    /// Name of the status.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Indicates whether the status is active.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Collection of bookings associated with this status.
    /// </summary>
    public virtual ICollection<Booking> Bookings { get; set; }
}
