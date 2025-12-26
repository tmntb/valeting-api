using System;
using Common.Enums;

namespace Service.Models.Role;

public class RoleDto
{
    /// <summary>
    /// Unique identifier of the role.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the role.
    /// </summary>
    public RoleEnum Name { get; set; }
}
