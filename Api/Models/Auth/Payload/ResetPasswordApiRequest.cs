namespace Api.Models.Auth.Payload;

/// <summary>
/// Contains the reset token and new password.
/// </summary>
public class ResetPasswordApiRequest
{
    /// <summary>
    /// One-time password reset JWT.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Replacement password.
    /// </summary>
    public string NewPassword { get; set; }
}
