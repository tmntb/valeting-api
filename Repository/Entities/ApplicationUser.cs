namespace Repository.Entities;

/// <summary>
/// Represents an application user with credentials for authentication.
/// </summary>
public partial class ApplicationUser
{
    public ApplicationUser()
    {
        CustomerBookings = new HashSet<Booking>();
        DecisionBookings = new HashSet<Booking>();
        RecoveryCodes = new HashSet<RecoveryCode>();
    }

    /// <summary>
    /// Unique identifier for the user.
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
    /// Foreign key referencing the role assigned to the user.
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Indicates whether the user account is active.
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

    /// <summary>
    /// Navigation property for the role assigned to the user.
    /// </summary>
    public virtual RdRole Role { get; set; } = null!;

    /// <summary>
    /// Collection of bookings associated with this user customer.
    /// </summary>
    public virtual ICollection<Booking> CustomerBookings { get; set; }

    /// <summary>
    /// Collection of bookings associated with this user admin.
    /// </summary>
    public virtual ICollection<Booking> DecisionBookings { get; set; }

    /// <summary>
    /// Collection of recovery codes associated with this user for MFA.
    /// </summary>
    public virtual ICollection<RecoveryCode> RecoveryCodes { get; set; }
}
