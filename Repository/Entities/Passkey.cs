namespace Repository.Entities;

public partial class Passkey
{
    /// <summary>
    /// Unique identifier of the passkey record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identifier of the user that owns this passkey.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// WebAuthn credential identifier provided by the browser during credential creation.
    /// </summary>
    public string CredentialId { get; set; }

    /// <summary>
    /// Public key used to verify future authenticator assertions for this passkey.
    /// </summary>
    public string PublicKey { get; set; }

    /// <summary>
    /// User-friendly name assigned to the passkey for display and management purposes.
    /// </summary>
    public string FriendlyName { get; set; }

    /// <summary>
    /// Optional authenticator GUID associated with the credential.
    /// </summary>
    public string? Aaguid { get; set; }

    /// <summary>
    /// Optional transport metadata describing how the authenticator communicates with the client.
    /// </summary>
    public string? Transports { get; set; }

    /// <summary>
    /// Timestamp when the passkey was registered.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp of the last successful authentication using this passkey.
    /// </summary>
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// Signature counter used to detect suspicious replay or cloning activity.
    /// </summary>
    public long? SignatureCounter { get; set; }

    /// <summary>
    /// Navigation property for the user that owns the passkey.
    /// </summary>
    public virtual ApplicationUser User { get; set; } = null!;
}
