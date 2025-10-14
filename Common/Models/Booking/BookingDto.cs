using Common.Models.Flexibility;
using Common.Models.VehicleSize;

namespace Common.Models.Booking;

public class BookingDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime BookingDate { get; set; }
    public FlexibilityDto Flexibility { get; set; } = new();
    public VehicleSizeDto VehicleSize { get; set; } = new();
    public int? ContactNumber { get; set; }
    public string Email { get; set; }
    public bool? Approved { get; set; }
}