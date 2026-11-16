namespace Common.Enums;

/// <summary>
/// Represents the different statuses that a booking can have in the system.
/// </summary>
public enum StatusEnum
{
    /// <summary>The booking is awaiting an administrator's decision.</summary>
    PENDING_APPROVAL,
    /// <summary>The booking has been accepted.</summary>
    APPROVED,
    /// <summary>The booking request has been declined.</summary>
    REJECTED,
    /// <summary>The booking has been cancelled.</summary>
    CANCELLED,
    /// <summary>The booking service has been completed.</summary>
    COMPLETED,
    /// <summary>The booking has passed its validity period without completion.</summary>
    EXPIRED
}
