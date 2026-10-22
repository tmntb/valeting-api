using Fido2NetLib;

namespace Service.Models.WebAuthn.Payload;

public class WebAuthnAuthenticationVerifyRequest
{
    public Guid CeremonyId { get; set; }
    public AuthenticatorAssertionRawResponse Credential { get; set; } = null!;
}
