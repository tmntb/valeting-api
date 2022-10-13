using Microsoft.EntityFrameworkCore;

using Valeting.Business;
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

        public async Task<IEnumerable<FlexibilityDTO>> ListAsync()
        {
            var flexibilityDTOs = new List<FlexibilityDTO>();

            var rdFlexibilities = await _valetingContext.RdFlexibilities.Where(x => x.Active).ToListAsync();
            if (rdFlexibilities == null)
                return flexibilityDTOs;

            flexibilityDTOs.AddRange(
                rdFlexibilities.Select(item => new FlexibilityDTO()
                {
                    Id = item.Id,
                    Description = item.Description,
                    Active= item.Active
                })
            );

            return flexibilityDTOs;
        }
    }
}
