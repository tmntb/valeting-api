using System.Security.Claims;

namespace Service.Models.Token.Payload;

/// <summary>
/// Contains the user and claims used to generate a JWT.
/// </summary>
public class GenerateTokenJwtDtoRequest
{
    /// <summary>
    /// Email address used to identify the user.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Token lifetime in minutes.
    /// </summary>
    public int Ttl { get; set; } = 60;

    /// <summary>
    /// Optional additional claims.
    /// </summary>
    public IEnumerable<Claim>? Claims { get; set; }
}