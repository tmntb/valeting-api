using Service.Models.Role;

namespace Service.Models.User;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Username used for login and identification.
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
    /// Role assigned to the user.
    /// </summary>
    public RoleDto Role { get; set; } = new();

    /// <summary>
    /// Indicates whether the user is active.
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
}
