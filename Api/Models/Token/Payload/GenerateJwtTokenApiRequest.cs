namespace Api.Models.Token.Payload;

/// <summary>
/// Represents the request body for generating a new Jwt access token.
/// </summary>
public class GenerateJwtTokenApiRequest
{
    /// <summary>
    /// Email of the user that is requesting a new Jwt access token.
    /// </summary>
    public string Email { get; set; }
}