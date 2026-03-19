using Common.Enums;
using Service.Models.Status;

namespace Service.Interfaces;

public interface IStatusRepository
{
    /// <summary>
    /// Retrieves a status record by its unique code from the database.
    /// </summary>
    /// <param name="statusCode">The unique code of the status.</param>
    /// <returns>A task that returns a <see cref="StatusDto"/> if found; otherwise, null.</returns>
    Task<StatusDto> GetByCodeAsync(StatusEnum statusCode);
}
