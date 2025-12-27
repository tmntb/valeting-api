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
    public string Username { get; set; }

    /// <summary>
    /// Hashed password hash of the user.
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    /// Contact number of the user.
    /// </summary>
    public int ContactNumber { get; set; }
    
    /// <summary>
    /// Email address of the user.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Foreign key referencing the role assigned to the user.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Indicates whether the user account is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Timestamp when the user account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the user account was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary> 
    /// Timestamp when the user last logged in.
    /// </summary>
    public DateTime LastLoginAt { get; set; }

    /// <summary>
    /// Navigation property for the role assigned to the user.
    /// </summary>
    public virtual RdRole Role { get; set; } = null!;
}
