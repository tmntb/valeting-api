using Fido2NetLib;

namespace Service.Models.WebAuthn.Payload;

/// <summary>
/// Represents the data required to verify a passkey registration
/// for a specific user.
/// </summary>
public class WebAuthnRegistrationVerifyDtoRequest
{
    /// <summary>
    /// Unique identifier of the user registering the passkey.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Unique identifier of the registration ceremony
    /// associated with the passkey registration process.
    /// </summary>
    public Guid CeremonyId { get; set; }

    /// <summary>
    /// Optional name assigned to the passkey
    /// to help the user identify it later.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Raw authenticator attestation response returned
    /// by the client after creating the passkey.
    /// </summary>
    public AuthenticatorAttestationRawResponse Credential { get; set; } = null!;
}
