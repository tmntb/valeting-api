using Microsoft.EntityFrameworkCore;
using Valeting.Common.Models.Flexibility;
using Valeting.Repository.Entities;
using Valeting.Repository.Interfaces;

namespace Valeting.Repository.Repositories;

public class FlexibilityRepository(ValetingContext valetingContext) : IFlexibilityRepository
{
    public async Task<List<FlexibilityDto>> GetFilteredAsync(FlexibilityFilterDto flexibilityFilterDto)
    {
        var initialList = await valetingContext.RdFlexibilities.ToListAsync();
        var listFlexibility = from rdFlexibility in initialList
                                where !flexibilityFilterDto.Active.HasValue || rdFlexibility.Active == flexibilityFilterDto.Active
                                select rdFlexibility;

        return new List<FlexibilityDto>();
    }

    public async Task<FlexibilityDto> GetByIdAsync(Guid id)
    {
        var rdFlexibility = await valetingContext.RdFlexibilities.FindAsync(id);
        if (rdFlexibility == null)
            return null;

        return new FlexibilityDto { };
    }
}