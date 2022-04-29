namespace DJValeting.Business
{
    public class BookingDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime BookingDate { get; set; }
        public FlexibilityDTO Flexibility { get; set; }
        public VehicleSizeDTO VehicleSize { get; set; }
        public int ContactNumber { get; set; }
        public string Email { get; set; }
        public bool? Approved { get; set; }
    }
}