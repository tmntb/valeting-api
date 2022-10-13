using Microsoft.EntityFrameworkCore;

using Valeting.Business;
using Valeting.Repositories.Entities;
using Valeting.Repositories.Interfaces;

namespace Valeting.Repositories
{
    public class VehicleSizeRepository : IVehicleSizeRepository
    {
        private readonly ValetingContext _valetingContext;

        public VehicleSizeRepository(ValetingContext valetingContext)
        {
            this._valetingContext = valetingContext;
        }

        public async Task<VehicleSizeDTO> FindByIDAsync(Guid id)
        {
            RdVehicleSize rdVehicleSize = await _valetingContext.RdVehicleSizes.FindAsync(id);
            if (rdVehicleSize == null)
                return null;

            var vehicleSizeDTO = new VehicleSizeDTO()
            {
                Id = id,
                Description = rdVehicleSize.Description,
                Active = rdVehicleSize.Active
            };

            return vehicleSizeDTO;
        }

        public async Task<IEnumerable<VehicleSizeDTO>> ListAsync()
        {
            var vehicleSizeDTOs = new List<VehicleSizeDTO>();

            var rdVehicleSizes = await _valetingContext.RdVehicleSizes.Where(x => x.Active).ToListAsync();
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
