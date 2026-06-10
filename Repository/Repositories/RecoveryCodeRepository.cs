using Repository.Entities;
using Service.Interfaces;
using Service.Models.Auth;

namespace Repository.Repositories;

public class RecoveryCodeRepository(ValetingContext valetingContext) : IRecoveryCodeRepository
{
    /// <inheritdoc />
    public async Task CreateManyAsync(IEnumerable<RecoveryCodeDto> recoveryCodesDto)
    {
        var recoveryCodes = recoveryCodesDto.Select(x => new RecoveryCode
        {
            Id = x.Id,
            UserId = x.User.Id,
            CodeHash = x.CodeHash,
            CreatedAt = x.CreatedAt,
            UsedAt = x.UsedAt
        });

        await valetingContext.RecoveryCodes.AddRangeAsync(recoveryCodes);
        await valetingContext.SaveChangesAsync();
    }
}
