namespace Valeting.Services.Objects.Booking;

public class BookingSV
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime BookingDate { get; set; }
    //public FlexibilitySV Flexibility { get; set; }
    //public VehicleSizeSV VehicleSize { get; set; }
    public int? ContactNumber { get; set; }
    public string Email { get; set; }
    public bool? Approved { get; set; }
}
