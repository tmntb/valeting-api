namespace Service.Models.WebAuthn;

public class PasskeyAuthenticationResult
{
    public bool Success { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
}
