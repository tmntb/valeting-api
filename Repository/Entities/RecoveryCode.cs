namespace Repository.Entities;

public class RecoveryCode
{
    /// <summary>
    /// Unique identifier for the recovery code.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key referencing the user associated with the recovery code.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Hash of the recovery code for MFA.
    /// </summary>
    public string CodeHash { get; set; }

    /// <summary>
    /// Indicates whether the recovery code has been used.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the recovery code was used, if applicable.
    /// </summary>
    public DateTime? UsedAt { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
}
