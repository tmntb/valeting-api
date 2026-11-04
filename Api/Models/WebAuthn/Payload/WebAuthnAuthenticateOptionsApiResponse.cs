using System.Text.Json;

namespace Api.Models.WebAuthn.Payload;

/// <summary>
/// Contains the WebAuthn options required by the client to authenticate.
/// </summary>
public class WebAuthnAuthenticateOptionsApiResponse
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
