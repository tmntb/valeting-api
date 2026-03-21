namespace Api.Models.User.Payload;

/// <summary>
/// Represents the request body for user login.
/// </summary>
public class LoginApiRequest
{
    /// <summary>
    /// The email address of the user attempting to log in.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The password of the user attempting to log in.
    /// </summary>
    public string Password { get; set; }
}
