using System.Text.Json;

namespace Service.Models.WebAuthn.Payload;

/// <summary>
/// Contains the options required by the client to begin WebAuthn credential registration.
/// </summary>
public class WebAuthnRegistrationOptionsDtoResponse
{
    /// <summary>
    /// Identifier of the registration ceremony.
    /// </summary>
    public Guid CeremonyId { get; set; }

    /// <summary>
    /// Public-key options to pass to the browser's WebAuthn API.
    /// </summary>
    public JsonElement PublicKey { get; set; }
}
