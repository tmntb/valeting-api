using Common.Enums;

namespace Service.Models.Role;

/// <summary>
/// Represents a data transfer object for a role record.
/// </summary>
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

    /// <summary>
    /// Name of the role.
    /// </summary>
    public string Name { get; set; }
}
