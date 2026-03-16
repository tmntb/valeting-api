using Common.Enums;

namespace Api.Models.Booking.Payload;

public class UpdateBookingApiStatusRequest
{
    public StatusEnum Status { get; set; }
}
