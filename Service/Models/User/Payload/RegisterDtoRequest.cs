using Common.Enums;

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

    /// <summary>
    /// The contact number for the new user.
    /// </summary>
    public int ContactNumber { get; set; }

    /// <summary>
    /// The email address for the new user.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The name of the role to be assigned to the new user.
    /// </summary>
    public RoleEnum RoleName { get; set; }
}
