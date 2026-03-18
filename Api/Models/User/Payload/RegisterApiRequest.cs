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
    [Required]
    public string Username { get; set; }

    /// <summary>
    /// Password of the user.
    /// </summary>
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// Contact number of the user.
    [Required]
    public int ContactNumber { get; set; }

    /// <summary>
    /// Email address of the user.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}