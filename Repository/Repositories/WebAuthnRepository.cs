using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.Auth;

namespace Repository.Repositories;

public class WebAuthnRepository(ValetingContext valetingContext) : IWebAuthnRepository
{
    /// <inheritdoc />
    public async Task CreateAsync(WebAuthnDto webAuthnDto, CancellationToken cancellationToken = default)
    {
        var passkey = new WebAuthn
        {
            Id = webAuthnDto.Id,
            UserId = webAuthnDto.UserId,
            CredentialId = webAuthnDto.CredentialId,
            PublicKey = webAuthnDto.PublicKey,
            FriendlyName = webAuthnDto.FriendlyName,
            Aaguid = webAuthnDto.Aaguid,
            Transports = webAuthnDto.Transports,
            CreatedAt = webAuthnDto.CreatedAt,
            LastUsedAt = webAuthnDto.LastUsedAt,
            SignatureCounter = webAuthnDto.SignatureCounter
        };

        await valetingContext.Passkeys.AddAsync(passkey, cancellationToken);
        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WebAuthnDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var passkeys = await valetingContext.Passkeys
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return passkeys.Select(FromEntity);
    }

    /// <inheritdoc />
    public async Task<WebAuthnDto?> GetByCredentialIdAsync(string credentialId, CancellationToken cancellationToken = default)
    {
        var passkey = await valetingContext.Passkeys.FirstOrDefaultAsync(x => x.CredentialId == credentialId, cancellationToken);

        return passkey == null ? null : FromEntity(passkey);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(WebAuthnDto webAuthnDto, CancellationToken cancellationToken = default)
    {
        var passkey = await valetingContext.Passkeys.FirstOrDefaultAsync(x => x.Id == webAuthnDto.Id && x.UserId == webAuthnDto.UserId, cancellationToken);

        if (passkey == null)
            return;

        passkey.CredentialId = webAuthnDto.CredentialId;
        passkey.PublicKey = webAuthnDto.PublicKey;
        passkey.FriendlyName = webAuthnDto.FriendlyName;
        passkey.Aaguid = webAuthnDto.Aaguid;
        passkey.Transports = webAuthnDto.Transports;
        passkey.LastUsedAt = webAuthnDto.LastUsedAt;
        passkey.SignatureCounter = webAuthnDto.SignatureCounter;

        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var passkey = await valetingContext.Passkeys.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (passkey == null)
            return;

        valetingContext.Passkeys.Remove(passkey);
        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Converts a Passkey entity to a PasskeyDto.
    /// </summary>
    /// <param name="webAuthn">The entity to convert.</param>
    /// <returns>The converted Dto.</returns>
    private static WebAuthnDto FromEntity(WebAuthn webAuthn) => new()
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
