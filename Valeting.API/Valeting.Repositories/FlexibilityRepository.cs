﻿using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Valeting.Repositories.Entities;
using Valeting.Repositories.Interfaces;

namespace Valeting.Repositories;

public class FlexibilityRepository(ValetingContext valetingContext, IMapper mapper) : IFlexibilityRepository
{
    // public async Task<FlexibilityListDTO> GetAsync(FlexibilityFilterDTO flexibilityFilterDTO)
    // {
    //     var flexibilityListDTO = new FlexibilityListDTO() { Flexibilities = new List<FlexibilityDTO>() };

    //     var initialList = await valetingContext.RdFlexibilities.ToListAsync();
    //     var listFlexibility = from rdFlexibility in initialList
    //                             where !flexibilityFilterDTO.Active.HasValue || rdFlexibility.Active == flexibilityFilterDTO.Active
    //                             select rdFlexibility;

    //     if (listFlexibility == null)
    //         return flexibilityListDTO;

    //     flexibilityListDTO.TotalItems = listFlexibility.Count();
    //     var nrPages = decimal.Divide(flexibilityListDTO.TotalItems, flexibilityFilterDTO.PageSize);
    //     flexibilityListDTO.TotalPages = (int)(nrPages - Math.Truncate(nrPages) > 0 ? Math.Truncate(nrPages) + 1 : Math.Truncate(nrPages));

    //     listFlexibility = listFlexibility.OrderBy(x => x.Id);
    //     listFlexibility = listFlexibility.Skip((flexibilityFilterDTO.PageNumber - 1) * flexibilityFilterDTO.PageSize).Take(flexibilityFilterDTO.PageSize);
    //     flexibilityListDTO.Flexibilities = mapper.Map<List<FlexibilityDTO>>(listFlexibility);
    //     return flexibilityListDTO;
    // }

    // public async Task<FlexibilityDTO> GetByIDAsync(Guid id)
    // {
    //     var rdFlexibility = await valetingContext.RdFlexibilities.FindAsync(id);
    //     if (rdFlexibility == null)
    //         return null;

    //     return new FlexibilityDTO()
    //     {
    //         Id = id,
    //         Description = rdFlexibility.Description,
    //         Active = rdFlexibility.Active
    //     };
    // }
}