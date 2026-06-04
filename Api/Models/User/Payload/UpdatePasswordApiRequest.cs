namespace Api.Models.User.Payload;

/// <summary>
/// Represents the request payload for updating a user's password.
/// </summary>
public class UpdatePasswordApiRequest
{
    /// <summary>
    /// New password for the user.
    /// </summary>
    public string Password { get; set; }
}
