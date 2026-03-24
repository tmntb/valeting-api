using System.ComponentModel.DataAnnotations;

namespace Api.Models.User.Payload;

/// <summary>
/// Represents the request body for user registration.
/// </summary>
public class RegisterApiRequest
{
    /// <summary>
    /// Email address of the user.
    /// </summary>
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// Password of the user.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// First name of the user
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Last name of the user
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Date of birth of the user
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Contact number of the user.
    public int ContactNumber { get; set; }
}