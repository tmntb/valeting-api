using Common.Enums;

namespace Service.Models.Status;

/// <summary>
/// Represents a data transfer object for a status record.
/// </summary>
public class StatusDto
{
    /// <summary>
    /// Unique identifier of the status.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Code representing the status.
    /// </summary>
    public StatusEnum Code { get; set; }

    /// <summary>
    /// Description of the status.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Indicates whether the status is active.
    /// </summary>
    public bool Active { get; set; }
}
