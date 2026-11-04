using Service.Models.User;

namespace Service.Models.Auth;

/// <summary>
/// Represents a recovery code associated with a user.
/// </summary>
public class RecoveryCodeDto
{
    /// <summary>
    /// Identifier of the recovery code.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User associated with the recovery code.
    /// </summary>
    public UserDto User { get; set; }

    /// <summary>
    /// Hash of the recovery code.
    /// </summary>
    public string CodeHash { get; set; }

    /// <summary>
    /// Date and time when the recovery code was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the recovery code was used, or <see langword="null"/> if it has not been used.
    /// </summary>
    public DateTime? UsedAt { get; set; }
}
