using Microsoft.EntityFrameworkCore;
using Common.Models.Flexibility;
using Service.Interfaces;
using Repository.Entities;

namespace Repository.Repositories;

public class FlexibilityRepository(ValetingContext valetingContext) : IFlexibilityRepository
{
    public async Task<List<FlexibilityDto>> GetFilteredAsync(FlexibilityFilterDto flexibilityFilterDto)
    {
        var initialList = await valetingContext.RdFlexibilities.ToListAsync();
        var listFlexibility = from rdFlexibility in initialList
                                where !flexibilityFilterDto.Active.HasValue || rdFlexibility.Active == flexibilityFilterDto.Active
                                select rdFlexibility;

        return listFlexibility.Select(x =>
            new FlexibilityDto
            {
                Id = x.Id,
                Description = x.Description,
                Active = x.Active
            }
        ).ToList();
    }

    public async Task<FlexibilityDto> GetByIdAsync(Guid id)
    {
        var rdFlexibility = await valetingContext.RdFlexibilities.FindAsync(id);
        if (rdFlexibility == null)
            return null;

        return new() 
        {
            Id = rdFlexibility.Id,
            Description = rdFlexibility.Description,
            Active = rdFlexibility.Active
        };
    }
}