using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Valeting.Business;
using Valeting.Repositories.Entities;
using Valeting.Repositories.Interfaces;

namespace Valeting.Repositories
{
    public class VehicleSizeRepository : IVehicleSizeRepository
    {
        private readonly DJValetingContext _valetingContext;

        public VehicleSizeRepository(IConfiguration configuration)
        {
            this._valetingContext = new DJValetingContext(
                new DbContextOptionsBuilder<DJValetingContext>().UseSqlServer(configuration.GetConnectionString("DJValetingConnection")).Options);
        }

        public async Task<VehicleSizeDTO> FindByIDAsync(Guid id)
        {
            RdVehicleSize rdVehicleSize = await _valetingContext.RdVehicleSizes.FindAsync(id);
            if (rdVehicleSize == null)
                return null;

            VehicleSizeDTO vehicleSizeDTO = new VehicleSizeDTO()
            {
                Id = id,
                Description = rdVehicleSize.Description,
                Active = rdVehicleSize.Active
            };

            return vehicleSizeDTO;
        }

        public async Task<IEnumerable<VehicleSizeDTO>> ListAsync()
        {
            List<VehicleSizeDTO> vehicleSizeDTOs = new List<VehicleSizeDTO>();

            List<RdVehicleSize> rdVehicleSizes = await _valetingContext.RdVehicleSizes.Where(x => x.Active).ToListAsync();
            if (rdVehicleSizes == null)
                return vehicleSizeDTOs;

            vehicleSizeDTOs.AddRange(
                rdVehicleSizes.Select(item => new VehicleSizeDTO()
                {
                    Id = item.Id,
                    Description = item.Description,
                    Active = item.Active
                })
            );

            return vehicleSizeDTOs;
        }
    }
}
