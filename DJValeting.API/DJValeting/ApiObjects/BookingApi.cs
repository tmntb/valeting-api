namespace DJValeting.ApiObjects
{
    public class BookingApi
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime BookingDate { get; set; }
        public FlexibilityApi Flexibility { get; set; }
        public VehicleSizeApi VehicleSize { get; set; }
        public int ContactNumber { get; set; }
        public string Email { get; set; }
        public bool? Approved { get; set; }
    }
}
