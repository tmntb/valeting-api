namespace Api.Models.Auth.Payload;

public class MfaRegenerateRecoveryCodesApiResponse
{
    /// <summary>
    /// The list of recovery codes for MFA account recovery.
    /// </summary>
    public List<string> RecoveryCodes { get; set; }
}
