namespace Api.Models.Security.Payload;

/// <summary>
/// Contains the recovery code to be validated.
/// </summary>
public class RecoveryCodeVerifyApiRequest
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