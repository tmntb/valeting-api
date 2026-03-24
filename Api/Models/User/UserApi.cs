using Api.Models.Role;

namespace Api.Models.User;

/// <summary>
/// Represents a user resource in the API.
/// </summary>
public class UserApi
{
    /// <summary>
    /// Email address of the user.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Contact number of the user.
    /// </summary>
    public int ContactNumber { get; set; }

    /// <summary>
    /// First name of the user.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Last name of the user.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Role assigned to the user.
    /// </summary>
    public RoleApi Role { get; set; }
}
