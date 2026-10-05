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

    /// <summary>
    /// Mfa code (optional).
    /// </summary>
    public string? MfaCode { get; set; }

    /// <summary>
    /// Recovery code (optional).
    /// </summary>
    public string? RecoveryCode { get; set; }

    /// <summary>
    /// User's new password.
    /// </summary>
    public string NewPassword { get; set; }
}
