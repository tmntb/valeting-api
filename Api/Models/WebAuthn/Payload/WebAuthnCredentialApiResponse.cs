namespace Api.Models.WebAuthn.Payload;

/// <summary>
/// Describes a WebAuthn credential registered to an account.
/// </summary>
public class WebAuthnCredentialApiResponse
{
    /// <summary>
    /// Identifier of the credential.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User-assigned name of the credential.
    /// </summary>
    public string FriendlyName { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the credential was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the credential was last used, if it has been used.
    /// </summary>
    public DateTime? LastUsedAt { get; set; }
}
