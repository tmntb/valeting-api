using Common.Enums;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.Role;

namespace Repository.Repositories;

public class RoleRepository(ValetingContext valetingContext) : IRoleRepository
{
    /// <inheritdoc />
    public async Task<RoleDto> GetByCodeAsync(RoleEnum RoleCode)
    {
        var rdRole = await valetingContext.RdRoles.FirstOrDefaultAsync(r => r.Code == RoleCode);
        if (rdRole == null)
            return null;

        return new()
        {
            Id = rdRole.Id,
            Code = rdRole.Code
        };
    }
}
