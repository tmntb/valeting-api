using Service.Models.RecoveryCode.Payload;

namespace Service.Interfaces;

/// <summary>
/// Defines operations for generating and managing account recovery codes.
/// </summary>
public interface IRecoveryCodeService
{
    /// <summary>
    /// Generate new recovery codes for a give user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>The user recovery codes.</returns>
    Task<List<string>> GenerateRecoveryCodesAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Verifies if the recovery code.
    /// </summary>
    /// <param name="verifyRecoveryCodeDtoRequest">The recovery code request payload.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>The user recovery codes.</returns>
    Task VerifyRecoveryCodeAsync(VerifyRecoveryCodeDtoRequest verifyRecoveryCodeDtoRequest, CancellationToken cancellationToken = default);
}
