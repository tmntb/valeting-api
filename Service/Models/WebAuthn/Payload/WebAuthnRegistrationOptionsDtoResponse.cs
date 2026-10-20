using System.Text.Json;

namespace Service.Models.WebAuthn.Payload;

public class WebAuthnRegistrationOptionsDtoResponse
{
    public Guid CeremonyId { get; set; }
    public JsonElement PublicKey { get; set; }
}
