namespace Api.Models.Security.Payload;

/// <summary>
/// Contains newly generated recovery codes for an authenticated user.
/// </summary>
public class RecoveryCodesGenerateApiResponse
{
    /// <summary>
    /// The list of recovery codes for MFA account recovery.
    /// </summary>
    public List<string> RecoveryCodes { get; set; }
}
