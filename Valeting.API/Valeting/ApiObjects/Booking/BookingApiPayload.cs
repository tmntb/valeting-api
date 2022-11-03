using Valeting.ApiObjects.Flexibility;
using Valeting.ApiObjects.VehicleSize;

namespace Valeting.ApiObjects.Booking
{
    public class CreateBookingApiRequest
    {
        public string Name { get; set; }
        public DateTime BookingDate { get; set; }
        public FlexibilityApi Flexibility { get; set; }
        public VehicleSizeApi VehicleSize { get; set; }
        public int? ContactNumber { get; set; }
        public string Email { get; set; }
    }

    public class CreateBookingApiResponse
    {
        public Guid Id { get; set; }
    }

    public class UpdateBookingApiRequest
    {
        public string Name { get; set; }
        public DateTime BookingDate { get; set; }
        public FlexibilityApi Flexibility { get; set; }
        public VehicleSizeApi VehicleSize { get; set; }
        public int? ContactNumber { get; set; }
        public string Email { get; set; }
        public bool? Approved { get; set; }
    }

    public class BookingApiPaginatedResponse : PaginationApi
    {
        public List<BookingApi> Bookings { get; set; }
    }

    public class BookingApiResponse
    {
        public BookingApi Booking { get; set; }
    }

    public class BookingApiError : ErrorApi { }
}

