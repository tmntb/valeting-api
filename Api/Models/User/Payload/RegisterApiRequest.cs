using System.ComponentModel.DataAnnotations;

namespace Api.Models.User.Payload;

public class RegisterApiRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public int ContactNumber { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}