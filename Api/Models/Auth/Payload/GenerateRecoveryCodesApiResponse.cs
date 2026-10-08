namespace Api.Models.Auth.Payload;

public class GenerateRecoveryCodesApiResponse
{
    /// <summary>
    /// The list of recovery codes for MFA account recovery.
    /// </summary>
    public List<string> RecoveryCodes { get; set; }
}
