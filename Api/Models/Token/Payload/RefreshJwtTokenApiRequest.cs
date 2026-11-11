namespace Api.Models.Token.Payload;

/// <summary>
/// Represents the request body for refreshing an authentication token.
/// </summary>
public class RefreshJwtTokenApiRequest
{
    /// <summary>
    /// The refresh token used to obtain a new authentication token.
    /// </summary>
    public string Token { get; set; } = default!;
}
