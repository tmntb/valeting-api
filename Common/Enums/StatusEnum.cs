namespace Common.Enums;

/// <summary>
/// Represents the different statuses that a booking can have in the system.
/// </summary>
public enum StatusEnum
{
    PENDING_APPROVAL,
    APPROVED,
    REJECTED,
    CANCELLED,
    COMPLETED,
    EXPIRED
}
