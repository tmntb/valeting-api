namespace Service.Models.Auth.Payload;

public class MfaEnableDtoRequest
{
    /// <summary>
    /// User's email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Mfa code
    /// </summary>
    public string MfaCode { get; set; }
}
