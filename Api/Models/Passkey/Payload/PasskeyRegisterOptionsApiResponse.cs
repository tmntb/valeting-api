using System.Text.Json;

namespace Api.Models.Passkey.Payload;

/// <summary>
/// Represents the response containing the options required to initiate a passkey registration ceremony.
/// </summary>
public class PasskeyRegisterOptionsApiResponse
{
	/// <summary>
	/// Unique identifier of the registration ceremony.
	/// Used to correlate the registration options with the subsequent verification request.
	/// </summary>
	public Guid CeremonyId { get; set; }

	/// <summary>
	/// Public key credential creation options required by the client to initiate
	/// the WebAuthn passkey registration ceremony.
	/// </summary>
	public JsonElement PublicKey { get; set; }
}