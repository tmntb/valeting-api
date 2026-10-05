using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.Auth;

namespace Repository.Repositories;

public class RecoveryCodeRepository(ValetingContext valetingContext) : IRecoveryCodeRepository
{
    /// <inheritdoc />
    public async Task CreateManyAsync(IEnumerable<RecoveryCodeDto> recoveryCodesDto, CancellationToken cancellationToken = default)
    {
        var recoveryCodes = recoveryCodesDto.Select(x => new RecoveryCode
        {
            Id = x.Id,
            UserId = x.User.Id,
            CodeHash = x.CodeHash,
            CreatedAt = x.CreatedAt,
            UsedAt = x.UsedAt
        });

        await valetingContext.RecoveryCodes.AddRangeAsync(recoveryCodes, cancellationToken);
        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteManyAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRecoveryCodes = await valetingContext.RecoveryCodes.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        valetingContext.RecoveryCodes.RemoveRange(userRecoveryCodes);
        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RecoveryCodeDto>> GetUserRecoveryCodesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRecoveryCodes = await valetingContext.RecoveryCodes.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        if(userRecoveryCodes == null || !userRecoveryCodes.Any())
        {
            return null;
        }
        
        return userRecoveryCodes.Select(rc => new RecoveryCodeDto
        {
            Id = rc.Id,
            CodeHash = rc.CodeHash,
            CreatedAt = rc.CreatedAt,
            User = new() { Id = rc.User.Id },
            UsedAt = rc.UsedAt
        });
    }

    /// <inheritdoc />
    public async Task UpdateUsedAtAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var recoveryCode = await valetingContext.RecoveryCodes.FirstOrDefaultAsync(rc => rc.Id == id, cancellationToken);
        if(recoveryCode == null)
        {
            return;
        }

        recoveryCode.UpdateUsedAt();

        await valetingContext.SaveChangesAsync(cancellationToken);
    }
}
