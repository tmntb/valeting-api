using System.ComponentModel.DataAnnotations;

namespace Api.Models.User.Payload;

public class LoginApiRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
