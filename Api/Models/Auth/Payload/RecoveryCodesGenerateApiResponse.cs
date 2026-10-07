namespace Api.Models.Auth.Payload;

public class RecoveryCodesGenerateApiResponse
{
    /// <summary>
    /// The list of recovery codes for MFA account recovery.
    /// </summary>
    public List<string> RecoveryCodes { get; set; }
}
