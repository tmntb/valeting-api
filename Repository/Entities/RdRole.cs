using Common.Enums;

namespace Repository.Entities;

/// <summary>
/// Represents a role that can be assigned to users for authorization purposes.
/// </summary>
public partial class RdRole
{
    /// <summary>
    /// Initializes a new instance of <see cref="RdRole"/> and its application users collection.
    /// </summary>
    public RdRole()
    {
        ApplicationUsers = new HashSet<ApplicationUser>();
    }

    /// <summary>
    /// Unique identifier for the role.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Code representing the role.
    /// </summary>
    public RoleEnum Code { get; set; }

    /// <summary>
    /// Name of the role.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Indicates whether the role is active.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Collection of application users associated with this role.
    /// </summary>
    public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
}
