namespace Api.Models.Auth.Payload;

public class MfaEnableApiRequest
{
    /// <summary>
    /// User email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Mfa code to enable
    /// </summary>
    public string MfaCode { get; set; }
}
