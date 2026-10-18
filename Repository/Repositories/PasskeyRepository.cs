using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.Auth;

namespace Repository.Repositories;

public class PasskeyRepository(ValetingContext valetingContext) : IPasskeyRepository
{
    /// <inheritdoc />
    public async Task AddAsync(PasskeyDto passkeyDto, CancellationToken cancellationToken = default)
    {
        var passkey = new Passkey
        {
            Id = passkeyDto.Id,
            UserId = passkeyDto.UserId,
            CredentialId = passkeyDto.CredentialId,
            PublicKey = passkeyDto.PublicKey,
            FriendlyName = passkeyDto.FriendlyName,
            Aaguid = passkeyDto.Aaguid,
            Transports = passkeyDto.Transports,
            CreatedAt = passkeyDto.CreatedAt,
            LastUsedAt = passkeyDto.LastUsedAt,
            SignatureCounter = passkeyDto.SignatureCounter
        };

        await valetingContext.Passkeys.AddAsync(passkey, cancellationToken);
        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PasskeyDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var passkeys = await valetingContext.Passkeys
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return passkeys.Select(FromEntity);
    }

    /// <inheritdoc />
    public async Task<PasskeyDto?> GetByCredentialIdAsync(string credentialId, CancellationToken cancellationToken = default)
    {
        var passkey = await valetingContext.Passkeys.FirstOrDefaultAsync(x => x.CredentialId == credentialId, cancellationToken);

        return passkey == null ? null : FromEntity(passkey);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(PasskeyDto passkeyDto, CancellationToken cancellationToken = default)
    {
        var passkey = await valetingContext.Passkeys.FirstOrDefaultAsync(x => x.Id == passkeyDto.Id && x.UserId == passkeyDto.UserId, cancellationToken);

        if (passkey == null)
            return;

        passkey.CredentialId = passkeyDto.CredentialId;
        passkey.PublicKey = passkeyDto.PublicKey;
        passkey.FriendlyName = passkeyDto.FriendlyName;
        passkey.Aaguid = passkeyDto.Aaguid;
        passkey.Transports = passkeyDto.Transports;
        passkey.LastUsedAt = passkeyDto.LastUsedAt;
        passkey.SignatureCounter = passkeyDto.SignatureCounter;

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
    /// <param name="passkey">The entity to convert.</param>
    /// <returns>The converted Dto.</returns>
    private static PasskeyDto FromEntity(Passkey passkey) => new()
    {
        Id = passkey.Id,
        UserId = passkey.UserId,
        CredentialId = passkey.CredentialId,
        PublicKey = passkey.PublicKey,
        FriendlyName = passkey.FriendlyName,
        Aaguid = passkey.Aaguid,
        Transports = passkey.Transports,
        CreatedAt = passkey.CreatedAt,
        LastUsedAt = passkey.LastUsedAt,
        SignatureCounter = passkey.SignatureCounter
    };
}
