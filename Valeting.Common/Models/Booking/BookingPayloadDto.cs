using Valeting.Common.Models.Flexibility;
using Valeting.Common.Models.VehicleSize;

namespace Valeting.Common.Models.Booking;

public class CreateBookingDtoRequest
{
    public string Name { get; set; }
    public DateTime BookingDate { get; set; }
    public FlexibilityDto Flexibility { get; set; }
    public VehicleSizeDto VehicleSize { get; set; }
    public int? ContactNumber { get; set; }
    public string Email { get; set; }
}

public class CreateBookingDtoResponse
{
    public Guid Id { get; set; }
}

public class UpdateBookingDtoRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime BookingDate { get; set; }
    public FlexibilityDto Flexibility { get; set; }
    public VehicleSizeDto VehicleSize { get; set; }
    public int? ContactNumber { get; set; }
    public string Email { get; set; }
    public bool? Approved { get; set; }
}

public class DeleteBookingDtoRequest
{
    public Guid Id { get; set; }
}

public class GetBookingDtoRequest
{
    public Guid Id { get; set; }
}

public class GetBookingDtoResponse
{
    public BookingDto Booking { get; set; }
}

public class PaginatedBookingDtoRequest
{
    public BookingFilterDto Filter { get; set; }
}

public class PaginatedBookingDtoResponse
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<BookingDto> Bookings { get; set; }
}