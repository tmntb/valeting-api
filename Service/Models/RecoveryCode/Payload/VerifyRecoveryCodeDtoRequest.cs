namespace Service.Models.RecoveryCode.Payload;

public class VerifyRecoveryCodeDtoRequest
{
    /// <summary>
    /// The email for the user.
    /// </summary>
    public string Email { get; set; }
    
    /// <summary>
    /// The recovery codes to be validated.
    /// </summary>
    public string Code { get; set; }
}