namespace Api.Models.Auth.Payload;

/// <summary>
/// Represents the request payload for forgot password process.
/// </summary>
public class ForgotPasswordApiRequest
{
    /// <summary>
    /// User's email.
    /// </summary>
    public string Email { get; set; }
}
