using Microsoft.EntityFrameworkCore;

using Valeting.Business.Flexibility;
using Valeting.Repositories.Entities;
using Valeting.Repositories.Interfaces;

namespace Valeting.Repositories
{
    public class FlexibilityRepository(ValetingContext valetingContext) : IFlexibilityRepository
    {
        public async Task<FlexibilityDTO> FindByIDAsync(Guid id)
        {
            var rdFlexibility = await valetingContext.RdFlexibilities.FindAsync(id);
            if (rdFlexibility == null)
                return null;

            var flexibilityDTO = new FlexibilityDTO()
            {
                Id = id,
                Description = rdFlexibility.Description,
                Active = rdFlexibility.Active
            };

            return flexibilityDTO;
        }

        public async Task<FlexibilityListDTO> ListAsync(FlexibilityFilterDTO flexibilityFilterDTO)
        {
            var flexibilityListDTO = new FlexibilityListDTO() { Flexibilities = new List<FlexibilityDTO>() };

            var initialList = await valetingContext.RdFlexibilities.ToListAsync();
            var listFlexibility = from rdFlexibility in initialList
                                  where !flexibilityFilterDTO.Active.HasValue || rdFlexibility.Active == flexibilityFilterDTO.Active
                                  select rdFlexibility;

            if (listFlexibility == null)
                return flexibilityListDTO;

            flexibilityListDTO.TotalItems = listFlexibility.Count();
            var nrPages = Decimal.Divide(flexibilityListDTO.TotalItems, flexibilityFilterDTO.PageSize);
            flexibilityListDTO.TotalPages = (int)(nrPages - Math.Truncate(nrPages) > 0 ? Math.Truncate(nrPages) + 1 : Math.Truncate(nrPages));

            listFlexibility = listFlexibility.OrderBy(x => x.Id);

            listFlexibility = listFlexibility.Skip((flexibilityFilterDTO.PageNumber - 1) * flexibilityFilterDTO.PageSize).Take(flexibilityFilterDTO.PageSize);

            flexibilityListDTO.Flexibilities.AddRange(
                listFlexibility.ToList().Select(item => new FlexibilityDTO()
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Active = item.Active
                    }
                ).ToList()
            );

            return flexibilityListDTO;
        }
    }
}
