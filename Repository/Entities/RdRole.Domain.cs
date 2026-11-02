using Service.Models.Role;

namespace Repository.Entities;

public partial class RdRole
{
    /// <summary>
    /// Creates a <see cref="RoleDto"/> from an <see cref="RdRole"/>.
    /// </summary>
    /// <param name="rdRole">The role entity.</param>
    /// <returns>A <see cref="RoleDto"/> representing the role.</returns>
    internal static RoleDto FromEntity(RdRole rdRole)
    {
        return new()
        {
            Id = rdRole.Id,
            Code = rdRole.Code
        };
    }
}