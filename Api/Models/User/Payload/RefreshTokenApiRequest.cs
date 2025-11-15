using System.ComponentModel.DataAnnotations;

namespace Api.Models.User.Payload;

public class RefreshTokenApiRequest
{
    [Required]
    public string Token { get; set; } = default!;
}
