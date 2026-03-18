namespace Api.Models.User.Payload;

/// <summary>
/// Represents the response body for a successful refresh token operation.
/// </summary>
public class RefreshTokenApiResponse
{
    /// <summary>
    /// New authentication token.
    /// </summary>
    public string Token { get; set; } = default!;

    /// <summary>
    /// Type of the authentication token.
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Expiry date of the authentication token.
    /// </summary>
    public DateTime ExpiryDate { get; set; }
}
