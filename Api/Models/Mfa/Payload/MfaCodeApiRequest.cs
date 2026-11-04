namespace Api.Models.Auth.Payload;

public class MfaCodeApiRequest
{
    /// <summary>
    /// Mfa code
    /// </summary>
    public string MfaCode { get; set; }
}
