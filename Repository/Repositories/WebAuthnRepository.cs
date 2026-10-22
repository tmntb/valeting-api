using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.Auth;
using Service.Models.WebAuthn;

namespace Repository.Repositories;

public class WebAuthnRepository(ValetingContext valetingContext) : IWebAuthnRepository
{
    /// <inheritdoc />
    public async Task CreateAsync(WebAuthnDto webAuthnDto, CancellationToken cancellationToken = default)
    {
        var webauth = new WebAuthn
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

        await valetingContext.WebAuthns.AddAsync(webauth, cancellationToken);
        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WebAuthnDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var webauths = await valetingContext.WebAuthns
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return webauths.Select(FromEntity);
    }

    /// <inheritdoc />
    public async Task<WebAuthnDto?> GetByCredentialIdAsync(string credentialId, CancellationToken cancellationToken = default)
    {
        var webauth = await valetingContext.WebAuthns.FirstOrDefaultAsync(x => x.CredentialId == credentialId, cancellationToken);

        return webauth == null ? null : FromEntity(webauth);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(WebAuthnDto webAuthnDto, CancellationToken cancellationToken = default)
    {
        var webauth = await valetingContext.WebAuthns.FirstOrDefaultAsync(x => x.Id == webAuthnDto.Id, cancellationToken);

        if (webauth == null)
            return;

        webauth.CredentialId = webAuthnDto.CredentialId;
        webauth.PublicKey = webAuthnDto.PublicKey;
        webauth.FriendlyName = webAuthnDto.FriendlyName;
        webauth.Aaguid = webAuthnDto.Aaguid;
        webauth.Transports = webAuthnDto.Transports;
        webauth.LastUsedAt = webAuthnDto.LastUsedAt;
        webauth.SignatureCounter = webAuthnDto.SignatureCounter;

        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateSignalCounter(WebAuthnDto webAuthnDto, CancellationToken cancellationToken = default)
    {
        var webauth = await valetingContext.WebAuthns.FirstOrDefaultAsync(x => x.Id == webAuthnDto.Id, cancellationToken);
        
        webauth.UpdateSignatureCounter(webAuthnDto.SignatureCounter);
        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var webauth = await valetingContext.WebAuthns.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (webauth == null)
            return;

        valetingContext.WebAuthns.Remove(webauth);
        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Converts a webauth entity to a webauthDto.
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
