using Service.Models.WebAuthn;

namespace Repository.Entities;

public partial class WebAuthn
{
    /// <summary>
    /// Creates a <see cref="WebAuthnDto"/> from an <see cref="WebAuthn"/>.
    /// </summary>
    /// <param name="webAuthn">The webauthn entity.</param>
    /// <returns>A <see cref="WebAuthnDto"/> representing the webauthn.</returns>
    internal static WebAuthnDto FromEntity(WebAuthn webAuthn)
    {
        return new()
        {
            Id = webAuthn.Id,
            UserId = webAuthn.UserId,
            CredentialId = webAuthn.CredentialId,
            PublicKey = webAuthn.PublicKey,
            FriendlyName = webAuthn.FriendlyName,
            Aaguid = webAuthn.Aaguid,
            Transports = webAuthn.Transports,
            CreatedAt = webAuthn.CreatedAt,
            LastUsedAt = webAuthn.LastUsedAt,
            SignatureCounter = webAuthn.SignatureCounter
        };
    }

    internal void UpdateSignatureCounter(long? signCount)
    {
        SignatureCounter = signCount;
        LastUsedAt = DateTime.UtcNow;
    }
}