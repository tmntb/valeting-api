namespace Service.Models.WebAuthn;

/// <summary>
/// Defines the types of webauthn ceremonies supported by the application.
/// </summary>
public enum WebAuthnCeremony
{
    /// <summary>
    /// Ceremony for registering a new webauthn.
    /// </summary>
    Registration,

    /// <summary>
    /// Ceremony for authenticating a user with a webauthn.
    /// </summary>
    Authentication
}

/// <summary>
/// Represents the state of a webauthn ceremony, including its challenge options, associated user information, and expiration time.
/// </summary>
public sealed class WebAuthnChallengeState
{
    /// <summary>
    /// Unique identifier of the ceremony.
    /// </summary>
    public Guid CeremonyId { get; init; }

    /// <summary>
    /// Type of webauthn ceremony being performed.
    /// </summary>
    public WebAuthnCeremony Ceremony { get; init; }

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
