using System;
using Common.Messages;
using Service.Helpers;
using Service.Interfaces;
using Service.Models.Auth;

namespace Service.Services;

public class RecoveryCodeService(IUserRepository userRepository, IRecoveryCodeRepository recoveryCodeRepository) : IRecoveryCodeService
{
    /// <inheritdoc />
    public async Task<List<string>> GenerateRecoveryCodesAsync(Guid userId)
    {
        var userDto = await userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException(Messages.NotFound);
        if (userDto.MfaSecret == null || !userDto.MfaEnabled)
        {
            throw new InvalidOperationException(Messages.MfaDisabled);
        }

        await recoveryCodeRepository.DeleteManyAsync(userDto.Id);

        var createDate = DateTime.UtcNow;

        var recoveryCodes = Enumerable.Range(0, 8)
                .Select(_ => GenerateCode())
                .ToList();

        var recoveryCodeDtos = recoveryCodes.Select(code => new RecoveryCodeDto
        {
            Id = Guid.NewGuid(),
            User = new() { Id = userDto.Id },
            CodeHash = HashHelper.GenerateHash(code),
            CreatedAt = createDate
        });

        await recoveryCodeRepository.CreateManyAsync(recoveryCodeDtos);

        return recoveryCodes;
    }

    /// <summary>
    /// Generates a single recovery code consisting of two sets of four digits separated by a hyphen (e.g., "1234-5678").
    /// </summary>
    /// <returns>A recovery code.</returns>
    private string GenerateCode()
    {
        return $"{Random.Shared.Next(1000, 9999)}-{Random.Shared.Next(1000, 9999)}";
    }
}
