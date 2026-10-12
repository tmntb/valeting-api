namespace Service.Interfaces;

public interface IRecoveryCodeService
{
    /// <summary>
    /// Generate new recovery codes for a give user
    /// </summary>
    /// <param name="userId">The user id</param>
    /// <returns>The user recovery codes.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the user doesn't exists.</exception>
    Task<List<string>> GenerateRecoveryCodesAsync(Guid userId);
}
