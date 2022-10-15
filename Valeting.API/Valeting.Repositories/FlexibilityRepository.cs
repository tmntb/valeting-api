using Microsoft.EntityFrameworkCore;

using Valeting.Business.Flexibility;
using Valeting.Repositories.Entities;
using Valeting.Repositories.Interfaces;

namespace Valeting.Repositories
{
    public class FlexibilityRepository : IFlexibilityRepository
    {
        private readonly ValetingContext _valetingContext;

        public FlexibilityRepository(ValetingContext valetingContext)
        {
            this._valetingContext = valetingContext;
        }

        public async Task<FlexibilityDTO> FindByIDAsync(Guid id)
        {
            var rdFlexibility = await _valetingContext.RdFlexibilities.FindAsync(id);
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

            var initialList = await _valetingContext.RdFlexibilities.ToListAsync();
            IEnumerable<RdFlexibility> listFlexibility = from rdFlexibility in initialList
                                                        where (!flexibilityFilterDTO.Active.HasValue || rdFlexibility.Active == flexibilityFilterDTO.Active)
                                                        select rdFlexibility;

            if (listFlexibility == null)
                return flexibilityListDTO;

            flexibilityListDTO.TotalItems = listFlexibility.Count();
            int nrPages = flexibilityListDTO.TotalItems / flexibilityFilterDTO.PageSize;
            flexibilityListDTO.TotalPages = nrPages < 1 ? 1 : nrPages;

            listFlexibility.OrderBy(x => x.Id);

            listFlexibility = listFlexibility.Skip((flexibilityFilterDTO.PageNumber - 1) * flexibilityFilterDTO.PageSize).Take(flexibilityFilterDTO.PageSize);

            flexibilityListDTO.Flexibilities.AddRange(
                listFlexibility.ToList().Select(item => new FlexibilityDTO()
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Active= item.Active
                    }
                ).ToList()
            );

            return flexibilityListDTO;
        }
    }
}
