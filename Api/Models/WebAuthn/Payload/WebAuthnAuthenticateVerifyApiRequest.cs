using Fido2NetLib;

namespace Api.Models.WebAuthn.Payload;

public class WebAuthnAuthenticateVerifyApiRequest
{
    public Guid CeremonyId { get; set; }
    public AuthenticatorAssertionRawResponse Credential { get; set; } = null!;
}
