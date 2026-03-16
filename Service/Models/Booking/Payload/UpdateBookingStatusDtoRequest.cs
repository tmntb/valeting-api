using Common.Enums;
using Service.Models.User;

namespace Service.Models.Booking.Payload;

public class UpdateBookingStatusDtoRequest
{
    public Guid Id { get; set; }
    public StatusEnum Status { get; set; }
    public StatusEnum CurrentStatus { get; set; }
    public UserDto UserDto { get; set; }
}
