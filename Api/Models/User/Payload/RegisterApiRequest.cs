using System.ComponentModel.DataAnnotations;

namespace Api.Models.User.Payload;

/// <summary>
/// Represents the request body for user registration.
/// </summary>
public class RegisterApiRequest
{
    /// <summary>
    /// Username of the user.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Password of the user.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Contact number of the user.
    public int ContactNumber { get; set; }

    /// <summary>
    /// Email address of the user.
    /// </summary>
    [EmailAddress]
    public string Email { get; set; }
}