using Fido2NetLib;

namespace Api.Models.WebAuthn.Payload;

/// <summary>
/// Contains the authenticator response and ceremony identifier used to complete WebAuthn authentication.
/// </summary>
public class WebAuthnAuthenticateVerifyApiRequest
{
    /// <summary>
    /// Identifier of the authentication ceremony being completed.
    /// </summary>
    public Guid CeremonyId { get; set; }

    /// <summary>
    /// Assertion returned by the authenticator.
    /// </summary>
    public AuthenticatorAssertionRawResponse Credential { get; set; } = null!;
}
