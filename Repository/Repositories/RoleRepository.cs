using Common.Enums;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.Role;

namespace Repository.Repositories;

public class RoleRepository(ValetingContext valetingContext) : IRoleRepository
{
    public async Task<RoleDto> GetByNameAsync(RoleEnum roleName)
    {
        var rdRole = await valetingContext.RdRoles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (rdRole == null)
            return null;

        return new RoleDto
        {
            Id = rdRole.Id,
            Name = rdRole.Name
        };
    }
}
