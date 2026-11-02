using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.Flexibility;
using Service.Models.Flexibility.Payload;

namespace Repository.Repositories;

public class FlexibilityRepository(ValetingContext valetingContext) : IFlexibilityRepository
{
    /// <inheritdoc />
    public async Task<FlexibilityDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rdFlexibility = await valetingContext.RdFlexibilities.FindAsync(id, cancellationToken);
        if (rdFlexibility == null)
            return null;

        return RdFlexibility.FromEntity(rdFlexibility);
    }

    /// <inheritdoc />
    public async Task<List<FlexibilityDto>> GetFilteredAsync(FlexibilityFilterDto flexibilityFilterDto, CancellationToken cancellationToken = default)
    {
        var initialList = await valetingContext.RdFlexibilities.ToListAsync(cancellationToken);
        var listFlexibility = from rdFlexibility in initialList
                                where !flexibilityFilterDto.Active.HasValue || rdFlexibility.Active == flexibilityFilterDto.Active
                                select rdFlexibility;

        return listFlexibility.Select(RdFlexibility.FromEntity).ToList();
    }
}