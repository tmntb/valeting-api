using Service.Models.Auth;

namespace Service.Interfaces;

public interface IRecoveryCodeRepository
{
    /// <summary>
    /// Adds a new recovery codes records to the database.
    /// </summary>
    /// <param name="recoveryCodesDto">The list of recovery codes data to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateManyAsync(IEnumerable<RecoveryCodeDto> recoveryCodesDto);

    /// <summary>
    /// Delete recovery codes records from the database.
    /// </summary>
    /// <param name="userId">Id of the user to delete recovery codes</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteManyAsync(Guid userId);
}
