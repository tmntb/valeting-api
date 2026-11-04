using Service.Models.VehicleSize;

namespace Repository.Entities;

public partial class RdVehicleSize
{
    /// <summary>
    /// Creates a <see cref="VehicleSizeDto"/> from an <see cref="RdVehicleSize"/>.
    /// </summary>
    /// <param name="rdVehicleSize">The vehicle size entity.</param>
    /// <returns>A <see cref="VehicleSizeDto"/> representing the vehicle size.</returns>
    internal static VehicleSizeDto FromEntity(RdVehicleSize rdVehicleSize)
    {
        return new()
        {
            Id = rdVehicleSize.Id,
            Name = rdVehicleSize.Name
        };
    } 
}