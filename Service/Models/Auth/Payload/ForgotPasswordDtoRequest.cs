namespace Service.Models.Auth.Payload;

/// <summary>
/// Contains the account recovery data required to set a new password.
/// </summary>
public class ForgotPasswordDtoRequest
{
    /// <summary>
    /// User's email.
    /// </summary>
    public string Email { get; set; }
}
