namespace Service.Models.Auth.Payload;

public class ForgotPasswordDtoRequest
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
