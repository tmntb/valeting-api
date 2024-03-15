namespace Valeting.Repositories.Entities;

public partial class Booking
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime BookingDate { get; set; }
    public Guid FlexibilityId { get; set; }
    public Guid VehicleSizeId { get; set; }
    public int ContactNumber { get; set; }
    public string Email { get; set; } = null!;
    public bool? Approved { get; set; }

    public virtual RdFlexibility Flexibility { get; set; } = null!;
    public virtual RdVehicleSize VehicleSize { get; set; } = null!;
}