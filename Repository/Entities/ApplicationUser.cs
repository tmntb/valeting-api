namespace Repository.Entities;

/// <summary>
/// Represents an application user with credentials for authentication.
/// </summary>
public partial class ApplicationUser
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Username of the user.
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// Hashed password of the user.
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Foreign key referencing the role assigned to the user.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Indicates whether the user account is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Navigation property for the role assigned to the user.
    /// </summary>
    public virtual RdRole Role { get; set; } = null!;
}
