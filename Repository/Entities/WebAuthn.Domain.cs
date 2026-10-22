namespace Repository.Entities;

public partial class WebAuthn
{
    internal void UpdateSignatureCounter(long? signCount)
    {
        SignatureCounter = signCount;
        LastUsedAt = DateTime.UtcNow;
    }
}