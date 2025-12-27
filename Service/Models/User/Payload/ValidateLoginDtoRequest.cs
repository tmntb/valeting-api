namespace Service.Models.User.Payload;

/// <summary>
/// Represents the request payload for validating a user's login credentials.
/// </summary>
public class ValidateLoginDtoRequest
{
    /// <summary>
    /// The email of the user attempting to log in.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The password of the user attempting to log in.
    /// </summary>
    public string Password { get; set; }
}
