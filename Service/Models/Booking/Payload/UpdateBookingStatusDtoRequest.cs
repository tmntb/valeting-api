using Common.Enums;
using Service.Models.User;

namespace Service.Models.Booking.Payload;

/// <summary>
/// Represents the data transfer object used to update the status of a booking.
/// </summary>
public class UpdateBookingStatusDtoRequest
{
    /// <summary>
    /// The unique identifier of the booking to be updated. This is a required field and must be a valid GUID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The new status to be assigned to the booking. This is a required field and must be a valid value from the <see cref="StatusEnum"/> enumeration.
    /// </summary>
    public StatusEnum Status { get; set; }

    /// <summary>
    /// The current status of the booking before the update. This is a required field and must be a valid value from the <see cref="StatusEnum"/> enumeration. 
    /// It is used for validation purposes to ensure that the booking is in the expected state before applying the status update.
    /// </summary>
    public StatusEnum CurrentStatus { get; set; }

    /// <summary>
    /// The user performing the status update. This is a required field and must contain valid user information, including the user's unique identifier and role. 
    /// It is used for authorization and auditing purposes to track who made the change and to ensure that the user has the necessary permissions to update the booking status.
    /// </summary>
    public UserDto UserDto { get; set; }
}
