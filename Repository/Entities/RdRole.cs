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
    /// Name of the role.
    /// </summary>
    public RoleEnum Name { get; set; }

    /// <summary>
    /// Collection of application users associated with this role.
    /// </summary>
    public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
}
