using Fido2NetLib;

namespace Service.Models.WebAuthn.Payload;

/// <summary>
/// Contains the authenticator assertion and ceremony identifier used to complete WebAuthn authentication.
/// </summary>
public class WebAuthnAuthenticationVerifyRequest
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
