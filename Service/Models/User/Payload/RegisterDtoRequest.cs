namespace Service.Models.User.Payload;

/// <summary>
/// Represents the request payload to register a new user.
/// </summary>
public class RegisterDtoRequest
{
    /// <summary>
    /// The username for the new user.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The password for the new user.
    /// </summary>
    public string Password { get; set; }
}
