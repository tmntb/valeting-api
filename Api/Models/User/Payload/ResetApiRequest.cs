namespace Api.Models.User.Payload;

public class ResetApiRequest
{
    /// <summary>
    /// User's email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// User's reset password.
    /// </summary>
    public string NewPassword { get; set; }
}
