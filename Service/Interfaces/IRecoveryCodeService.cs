namespace Service.Interfaces;

public interface IRecoveryCodeService
{
    /// <summary>
    /// Generate new recovery codes for a give user
    /// </summary>
    /// <param name="userId">The user id</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>The user recovery codes.</returns>
    Task<List<string>> GenerateRecoveryCodesAsync(Guid userId, CancellationToken cancellationToken = default);
}
