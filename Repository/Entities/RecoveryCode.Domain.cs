namespace Repository.Entities;

public partial class RecoveryCode
{
    /// <summary>
    /// Updates the recovery code used at timestamp to the current UTC time.
    /// </summary>
    internal void UpdateUsedAt()
    {
        UsedAt = DateTime.UtcNow;
    }
}
