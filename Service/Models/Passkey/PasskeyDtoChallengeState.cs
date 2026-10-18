namespace Service.Models.Passkey;

/// <summary>
/// Defines the types of passkey ceremonies supported by the application.
/// </summary>
public enum PasskeyCeremony
{
    /// <summary>
    /// Ceremony for registering a new passkey.
    /// </summary>
    Registration,

    /// <summary>
    /// Ceremony for authenticating a user with a passkey.
    /// </summary>
    Authentication
}

/// <summary>
/// Represents the state of a passkey ceremony, including its challenge
/// options, associated user information, and expiration time.
/// </summary>
public sealed class PasskeyChallengeState
{
    /// <summary>
    /// Unique identifier of the ceremony.
    /// </summary>
    public Guid CeremonyId { get; init; }

    /// <summary>
    /// Type of passkey ceremony being performed.
    /// </summary>
    public PasskeyCeremony Ceremony { get; init; }

    /// <summary>
    /// Optional identifier of the user associated with the ceremony.
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Optional email address associated with the ceremony.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Serialized JSON representation of the options generated for the ceremony.
    /// </summary>
    public string OptionsJson { get; init; } = string.Empty;

    /// <summary>
    /// Date and time when the ceremony expires.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; init; }
}
