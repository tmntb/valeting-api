using Fido2NetLib;

namespace Api.Models.WebAuthn.Payload;

/// <summary>
/// Represents the request used to verify a passkey registration ceremony.
/// </summary>
public class WebauthnRegisterVerifyApiRequest
{
    /// <summary>
    /// Optional name assigned to the passkey
    /// to help the user identify it later.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Unique identifier of the registration ceremony initiated when the passkey registration options were requested.
    /// </summary>
    public Guid CeremonyId { get; set; }

    /// <summary>
    /// Raw authenticator attestation response returned
    /// by the client after creating the passkey.
    /// </summary>
    public AuthenticatorAttestationRawResponse Credential { get; set; } = null!;
}