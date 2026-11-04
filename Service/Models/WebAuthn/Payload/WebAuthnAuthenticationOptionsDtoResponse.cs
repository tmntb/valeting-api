using System.Text.Json;

namespace Service.Models.WebAuthn.Payload;

/// <summary>
/// Contains the options required by the client to begin WebAuthn authentication.
/// </summary>
public class WebAuthnAuthenticationOptionsDtoResponse
{
    /// <summary>
    /// Identifier of the authentication ceremony.
    /// </summary>
    public Guid CeremonyId { get; set; }

    /// <summary>
    /// Public-key options to pass to the browser's WebAuthn API.
    /// </summary>
    public JsonElement PublicKey { get; set; }
}
