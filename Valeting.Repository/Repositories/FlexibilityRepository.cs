using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Valeting.Repository.Entities;
using Valeting.Repository.Interfaces;
using Valeting.Common.Models.Flexibility;

namespace Valeting.Repository.Repositories;

public class FlexibilityRepository(ValetingContext valetingContext, IMapper mapper) : IFlexibilityRepository
{
    public async Task<FlexibilityListDto> GetAsync(FlexibilityFilterDto flexibilityFilterDto)
    {
        var flexibilityListDto = new FlexibilityListDto() { Flexibilities = [] };

        var initialList = await valetingContext.RdFlexibilities.ToListAsync();
        var listFlexibility = from rdFlexibility in initialList
                                where !flexibilityFilterDto.Active.HasValue || rdFlexibility.Active == flexibilityFilterDto.Active
                                select rdFlexibility;

        if (listFlexibility == null)
            return flexibilityListDto;

        flexibilityListDto.TotalItems = listFlexibility.Count();
        var nrPages = decimal.Divide(flexibilityListDto.TotalItems, flexibilityFilterDto.PageSize);
        flexibilityListDto.TotalPages = (int)(nrPages - Math.Truncate(nrPages) > 0 ? Math.Truncate(nrPages) + 1 : Math.Truncate(nrPages));

        listFlexibility = listFlexibility.OrderBy(x => x.Id);
        listFlexibility = listFlexibility.Skip((flexibilityFilterDto.PageNumber - 1) * flexibilityFilterDto.PageSize).Take(flexibilityFilterDto.PageSize);
        flexibilityListDto.Flexibilities = mapper.Map<List<FlexibilityDto>>(listFlexibility);
        return flexibilityListDto;
    }

    public async Task<FlexibilityDto> GetByIdAsync(Guid id)
    {
        var rdFlexibility = await valetingContext.RdFlexibilities.FindAsync(id);
        if (rdFlexibility == null)
            return null;

        return new FlexibilityDto()
        {
            Id = id,
            Description = rdFlexibility.Description,
            Active = rdFlexibility.Active
        };
    }
}