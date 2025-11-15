namespace Service.Models.User.Payload;

/// <summary>
/// Represents the response returned when a JWT token is generated.
/// </summary>
public class GenerateTokenJWTDtoResponse
{
    /// <summary>
    /// The generated JWT token string.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// The date and time when the token expires.
    /// </summary>
    public DateTime ExpiryDate { get; set; }

    /// <summary>
    /// The type of token, typically "Bearer".
    /// </summary>
    public string TokenType { get; set; }
}
