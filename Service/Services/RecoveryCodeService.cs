using Common.Messages;
using Service.Helpers;
using Service.Interfaces;
using Service.Models.RecoveryCode;
using Service.Models.RecoveryCode.Payload;
using Service.Validators.RecoveryCode;
using Service.Validators.Utils;

namespace Service.Services;

public class RecoveryCodeService(IUserRepository userRepository, IRecoveryCodeRepository recoveryCodeRepository) : IRecoveryCodeService
{
    /// <inheritdoc />
    public async Task<List<string>> GenerateRecoveryCodesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userDto = await userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new KeyNotFoundException(Messages.NotFound);
        if (userDto.MfaSecret == null || !userDto.TwoFactorAuthEnabled)
        {
            throw new InvalidOperationException(Messages.MfaDisabled);
        }

        await recoveryCodeRepository.DeleteManyAsync(userDto.Id, cancellationToken);

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

        await recoveryCodeRepository.CreateManyAsync(recoveryCodeDtos, cancellationToken);

        return recoveryCodes;
    }
    
    /// <inheritdoc />
    public async Task VerifyRecoveryCodeAsync(VerifyRecoveryCodeDtoRequest verifyRecoveryCodeDtoRequest, CancellationToken cancellationToken = default)
    {
        verifyRecoveryCodeDtoRequest.ValidateRequest(new RecoveryCodeVerifyValidator());
        
        var userDto = await userRepository.GetByEmailAsync(verifyRecoveryCodeDtoRequest.Email, cancellationToken);
        
        var userRecoveryCodes = await recoveryCodeRepository.GetUserRecoveryCodesAsync(userDto.Id, cancellationToken);
        if (userRecoveryCodes == null || !userRecoveryCodes.Any())
        {
            throw new InvalidOperationException(Messages.NoRecoveryCodesForUser);
        }

        var recoveryCode = userRecoveryCodes.SingleOrDefault(rc => rc.UsedAt == null && BCrypt.Net.BCrypt.Verify(verifyRecoveryCodeDtoRequest.Code, rc.CodeHash))
                           ?? throw new InvalidOperationException(Messages.InvalidRecoveryCode);

        await recoveryCodeRepository.UpdateUsedAtAsync(recoveryCode.Id, cancellationToken);
    }

    /// <summary>
    /// Generates a single recovery code consisting of two sets of four digits separated by a hyphen (e.g., "1234-5678").
    /// </summary>
    /// <returns>A recovery code.</returns>
    private static string GenerateCode() => $"{Random.Shared.Next(1000, 9999)}-{Random.Shared.Next(1000, 9999)}";
}
