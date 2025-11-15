namespace Service.Models.User;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Username used for login and identification.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Hashed password of the user.
    /// </summary>
    public string Password { get; set; }
}
