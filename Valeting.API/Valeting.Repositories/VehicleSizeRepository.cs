using Microsoft.EntityFrameworkCore;

using Valeting.Business.VehicleSize;
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
            var rdVehicleSize = await _valetingContext.RdVehicleSizes.FindAsync(id);
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

        public async Task<VehicleSizeListDTO> ListAsync(VehicleSizeFilterDTO vehicleSizeFilterDTO)
        {
            var vehicleSizeListDTO = new VehicleSizeListDTO() { VehicleSizes = new List<VehicleSizeDTO>() };

            var initialList = await _valetingContext.RdVehicleSizes.ToListAsync();
            IEnumerable<RdVehicleSize> listVehicleSize = from rdVehicleSize in initialList
                                                         where (!vehicleSizeFilterDTO.Active.HasValue || rdVehicleSize.Active == vehicleSizeFilterDTO.Active)
                                                         select rdVehicleSize;

            if (listVehicleSize == null)
                return vehicleSizeListDTO;

            vehicleSizeListDTO.TotalItems = listVehicleSize.Count();
            var nrPages = Decimal.Divide(vehicleSizeListDTO.TotalItems, vehicleSizeFilterDTO.PageSize);
            vehicleSizeListDTO.TotalPages = (int)(nrPages - Math.Truncate(nrPages) > 0 ? Math.Truncate(nrPages) + 1 : Math.Truncate(nrPages));

            listVehicleSize.OrderBy(x => x.Id);

            listVehicleSize = listVehicleSize.Skip((vehicleSizeFilterDTO.PageNumber - 1) * vehicleSizeFilterDTO.PageSize).Take(vehicleSizeFilterDTO.PageSize);

            vehicleSizeListDTO.VehicleSizes.AddRange(
                listVehicleSize.ToList().Select(item => new VehicleSizeDTO()
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Active = item.Active
                    }
                ).ToList()
            );

            return vehicleSizeListDTO;
        }
    }
}
