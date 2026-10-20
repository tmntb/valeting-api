using Fido2NetLib;

namespace Service.Models.Passkey;

public class PasskeyAuthenticationVerifyRequest
{
    public Guid CeremonyId { get; set; }
    public AuthenticatorAssertionRawResponse Credential { get; set; } = null!;
}
