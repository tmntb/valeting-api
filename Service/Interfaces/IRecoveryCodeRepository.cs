using Service.Models.Auth;

namespace Service.Interfaces;

public interface IRecoveryCodeRepository
{
    /// <summary>
    /// Adds a new recovery codes records to the database.
    /// </summary>
    /// <param name="recoveryCodesDto">The list of recovery codes data to create.</param>
    /// <param name="cancellationToken">Cancelation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateManyAsync(IEnumerable<RecoveryCodeDto> recoveryCodesDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete recovery codes records from the database.
    /// </summary>
    /// <param name="userId">Id of the user to delete recovery codes.</param>
    /// <param name="cancellationToken">Cancelation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteManyAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves the recovery codes for a specific user from the database.
    /// </summary>
    /// <param name="userId">Id of the user to delete recovery codes.</param>
    /// <param name="cancellationToken">Cancelation token.</param>
    /// <returns>List of recovery codes for the given user.</returns>
    Task<IEnumerable<RecoveryCodeDto>> GetUserRecoveryCodesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user at datetime for a specific recovery code in the database.
    /// </summary>
    /// <param name="id">Id of the recovery code to update the user at field.</param>
    /// <param name="cancellationToken">Cancelation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateUsedAtAsync(Guid id, CancellationToken cancellationToken = default);
}
