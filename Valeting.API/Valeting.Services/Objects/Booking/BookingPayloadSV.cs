using Valeting.Services.Objects.Core;

namespace Valeting.Services.Objects.Booking;

public class CreateBookingSVRequest
{
    public string Name { get; set; }
    public DateTime BookingDate { get; set; }
    //public FlexibilitySV Flexibility { get; set; }
    //public VehicleSizeSV VehicleSize { get; set; }
    public int? ContactNumber { get; set; }
    public string Email { get; set; }
}

public class CreateBookingSVResponse : ValetingOutputSV
{
    public Guid Id { get; set; }
}

public class UpdateBookingSVRequest
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

public class UpdateBookingSVResponse : ValetingOutputSV { }

public class DeleteBookingSVRequest
{
    public Guid Id { get; set; }
}

public class DeleteBookingSVResponse : ValetingOutputSV { }

public class GetBookingSVRequest
{
    public Guid Id { get; set; }
}

public class GetBookingSVResponse : ValetingOutputSV
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

public class PaginatedBookingSVRequest
{
    public BookingFilterSV Filter { get; set; }
}

public class PaginatedBookingSVResponse : ValetingOutputSV
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<BookingSV> Bookings { get; set; }
}