using Common.Enums;

namespace Api.Models.Token.Payload;

/// <summary>
/// Represents the response body for a successful token operation.
/// </summary>
public class JwtTokenApiResponse
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

    /// <summary>
    /// Role assigned to the authenticated user.
    /// </summary>
    public RoleEnum Role { get; set; }
}
