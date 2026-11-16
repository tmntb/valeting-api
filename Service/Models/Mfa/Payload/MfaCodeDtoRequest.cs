namespace Service.Models.Mfa.Payload;

/// <summary>
/// Contains a user's multi-factor authentication code for verification.
/// </summary>
public class MfaCodeDtoRequest
{
    /// <summary>
    /// User id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Mfa code
    /// </summary>
    public string MfaCode { get; set; }
}
