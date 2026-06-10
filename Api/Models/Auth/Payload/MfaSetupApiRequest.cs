using System.ComponentModel.DataAnnotations;

namespace Api.Models.Auth.Payload;

public class MfaSetupApiRequest
{
    /// <summary>
    /// Users email
    /// </summary>
    public string Email { get; set; }
}
