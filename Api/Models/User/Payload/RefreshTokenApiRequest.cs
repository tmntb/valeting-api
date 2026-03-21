namespace Api.Models.User.Payload;

/// <summary>
/// Represents the request body for refreshing an authentication token.
/// </summary>
public class RefreshTokenApiRequest
{
    /// <summary>
    /// The refresh token used to obtain a new authentication token.
    /// </summary>
    public string Token { get; set; } = default!;
}
