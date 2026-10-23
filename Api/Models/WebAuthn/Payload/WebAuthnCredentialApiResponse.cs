namespace Api.Models.WebAuthn.Payload;

public class WebAuthnCredentialApiResponse
{
    public Guid Id { get; set; }
    public string FriendlyName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
