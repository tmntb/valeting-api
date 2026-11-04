using Service.Models.Flexibility;

namespace Repository.Entities;

public partial class RdFlexibility
{
    /// <summary>
    /// Creates a <see cref="FlexibilityDto"/> from an <see cref="RdFlexibility"/>.
    /// </summary>
    /// <param name="rdFlexibility">The flexibility entity.</param>
    /// <returns>A <see cref="FlexibilityDto"/> representing the flexibility.</returns>
    internal static FlexibilityDto FromEntity(RdFlexibility rdFlexibility)
    {
        return new()
        {
            Id = rdFlexibility.Id,
            Name = rdFlexibility.Name,
            NumberOfMinutes = rdFlexibility.NumberOfMinutes,
            Active = rdFlexibility.Active
        };
    }
}