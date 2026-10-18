using System.Text.Json;

namespace Service.Models.Passkey.Payload;

public class PasskeyRegistrationOptionsDtoResponse
{
    public Guid CeremonyId { get; set; }
    public JsonElement PublicKey { get; set; }
}
