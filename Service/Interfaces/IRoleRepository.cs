using Common.Enums;
using Service.Models.Role;

namespace Service.Interfaces;

public interface IRoleRepository
{
    Task<RoleDto> GetByNameAsync(RoleEnum roleName);
}
