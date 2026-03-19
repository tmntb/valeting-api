using Api.Models.Role;

namespace Api.Models.User;

/// <summary>
/// Represents a user resource in the API.
/// </summary>
public class UserApi
{
    /// <summary>
    /// Username used for login and identification.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Contact number of the user.
    /// </summary>
    public int ContactNumber { get; set; }

    /// <summary>
    /// Email address of the user.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Role assigned to the user.
    /// </summary>
    public RoleApi Role { get; set; }
}
