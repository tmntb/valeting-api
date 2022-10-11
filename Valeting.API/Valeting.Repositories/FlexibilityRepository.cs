using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Valeting.Business;
using Valeting.Repositories.Entities;
using Valeting.Repositories.Interfaces;

namespace Valeting.Repositories
{
    public class FlexibilityRepository : IFlexibilityRepository
    {
        private readonly ValetingContext _valetingContext;

        public FlexibilityRepository(IConfiguration configuration)
        {
            this._valetingContext = new ValetingContext(
                new DbContextOptionsBuilder<ValetingContext>().UseSqlServer(configuration.GetConnectionString("DJValetingConnection")).Options);
        }

        public async Task<FlexibilityDTO> FindByIDAsync(Guid id)
        {
            RdFlexibility rdFlexibility = await _valetingContext.RdFlexibilities.FindAsync(id);
            if (rdFlexibility == null)
                return null;

            FlexibilityDTO flexibilityDTO = new FlexibilityDTO()
            {
                Id = id,
                Description = rdFlexibility.Description,
                Active = rdFlexibility.Active
            };

            return flexibilityDTO;
        }

        public async Task<IEnumerable<FlexibilityDTO>> ListAsync()
        {
            List<FlexibilityDTO> flexibilityDTOs = new List<FlexibilityDTO>();

            List<RdFlexibility> rdFlexibilities = await _valetingContext.RdFlexibilities.Where(x => x.Active).ToListAsync();
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
