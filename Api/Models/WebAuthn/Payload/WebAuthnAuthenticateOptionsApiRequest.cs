namespace Api.Models.WebAuthn.Payload;

/// <summary>
/// Contains the account identifier used to begin a WebAuthn authentication ceremony.
/// </summary>
public class WebAuthnAuthenticateOptionsApiRequest
{
    /// <summary>
    /// Email address of the account to authenticate.
    /// </summary>
    public string Email { get; init; }
}
