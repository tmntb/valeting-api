namespace Service.Models.Auth;

public class PasskeyDto
{
    /// <summary>
    /// Unique identifier of the passkey record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identifier of the user who owns the passkey.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// WebAuthn credential identifier returned by the browser during registration.
    /// </summary>
    public string CredentialId { get; set; } = string.Empty;

    /// <summary>
    /// Public key used to verify WebAuthn authenticator responses.
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Friendly label displayed to the user for this passkey.
    /// </summary>
    public string FriendlyName { get; set; } = "Passkey";

    /// <summary>
    /// Optional authenticator GUID that identifies the device or platform.
    /// </summary>
    public string? Aaguid { get; set; }

    /// <summary>
    /// Optional transport hints supported by the authenticator, such as USB or internal.
    /// </summary>
    public string? Transports { get; set; }

    /// <summary>
    /// Timestamp when the passkey was first registered.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp of the last successful passkey authentication, if available.
    /// </summary>
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// Signature counter value used to detect cloned or replayed authenticator state.
    /// </summary>
    public long? SignatureCounter { get; set; }
}
