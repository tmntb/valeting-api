using Service.Models.Status;

namespace Repository.Entities;

public partial class RdStatus
{
    /// <summary>
    /// Creates a <see cref="StatusDto"/> from an <see cref="RdStatus"/>.
    /// </summary>
    /// <param name="rdStatus">The status entity.</param>
    /// <returns>A <see cref="StatusDto"/> representing the status.</returns>
    internal static StatusDto FromEntity(RdStatus rdStatus)
    {
        return new()
        {
            Id = rdStatus.Id,
            Code = rdStatus.Code,
            Name = rdStatus.Name
        };
    }
}