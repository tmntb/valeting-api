namespace Api.Models.User.Payload;

/// <summary>
/// Represents the response body for a successful user login, containing the authentication token and its expiry information.
/// </summary>
public class LoginApiResponse
{
    /// <summary>
    /// Authentication token.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Expiry date of the authentication token.
    /// </summary>
    public DateTime ExpiryDate { get; set; }

    /// <summary>
    /// Type of the authentication token.
    /// </summary>
    public string TokenType { get; set; }
}
