namespace Api.Models.Auth.Payload;

/// <summary>
/// Contains newly generated recovery codes for an authenticated user.
/// </summary>
public class GenerateRecoveryCodesApiResponse
{
    /// <summary>
    /// The list of recovery codes for MFA account recovery.
    /// </summary>
    public List<string> RecoveryCodes { get; set; }
}
