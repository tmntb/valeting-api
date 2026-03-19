using Common.Enums;
using Service.Models.Role;

namespace Service.Interfaces;

public interface IRoleRepository
{
    /// <summary>
    /// Retrieves a role record by its unique code from the database.
    /// </summary>
    /// <param name="roleCode">The unique code of the role.</param>
    /// <returns>A task that returns a <see cref="RoleDto"/> if found; otherwise, null.</returns>
    Task<RoleDto> GetByCodeAsync(RoleEnum roleCode);
}
