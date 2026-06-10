using Service.Models.Auth;
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
    /// Email address of the user.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Hashed password hash of the user.
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    /// Plain text password used for registration and login validation.
    /// This property should not be stored in the database and is only used for input validation.
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    /// First name of the user
    /// </summary>
    public string FirstName { get; set; }
    
    /// <summary>
    /// Last name of the user
    /// </summary>
    public string LastName { get; set; }
    
    /// <summary>
    /// Date of birth of the user
    /// </summary>
    public DateOnly DateOfBirth { get; set; }
    
    /// <summary>
    /// Contact number of the user.
    /// </summary>
    public int ContactNumber { get; set; }

    /// <summary>
    /// Role assigned to the user.
    /// </summary>
    public RoleDto Role { get; set; } = new();

    /// <summary>
    /// Indicates whether the user is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Indicates whether multi-factor authentication (MFA) is enabled for the user.
    /// </summary>
    public bool MfaEnabled { get; set; }

    /// <summary> 
    /// Secret key used for generating MFA codes, if MFA is enabled.
    /// </summary>
    public string? MfaSecret { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public List<RecoveryCodeDto> RecoveryCodes { get; set; }
    
    /// <summary>
    /// Timestamp when the user account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the user account was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary> 
    /// Timestamp when the user last logged in.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}
