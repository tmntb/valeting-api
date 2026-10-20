using System.Text.Json;

namespace Api.Models.WebAuthn.Payload;

public class WebAuthnAuthenticateOptionsApiResponse
{
    public Guid CeremonyId { get; set; }
    public JsonElement PublicKey { get; set; }
}
