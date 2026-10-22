namespace Service.Models.Mfa.Payload;

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
