using Common.Enums;

namespace Service.Models.Role;

public class RoleDto
{
    /// <summary>
    /// Unique identifier of the role.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Code of the role.
    /// </summary>
    public RoleEnum Code { get; set; }
}
