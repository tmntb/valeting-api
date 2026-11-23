namespace Api.Models.Security.Payload;

/// <summary>
/// Contains the one-time MFA code submitted for verification.
/// </summary>
public class MfaCodeApiRequest
{
    /// <summary>
    /// Mfa code
    /// </summary>
    public string MfaCode { get; set; }
}
