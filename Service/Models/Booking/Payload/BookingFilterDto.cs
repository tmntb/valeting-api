using Common.Enums;
using Service.Models.Core;

namespace Service.Models.Booking.Payload;

/// <summary>
/// Represents the filter parameters used when querying bookings with pagination support.
/// Inherits common filtering properties from <see cref="FilterDto"/> such as PageNumber and PageSize.
/// </summary>
public class BookingFilterDto : FilterDto
{
    /// <summary>
    /// Optional filter to retrieve bookings for a specific customer by their unique identifier.
    /// If provided, only bookings associated with the specified customer will be returned.
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Optional filter to retrieve bookings with a specific status.
    /// If provided, only bookings matching the specified status will be returned.
    /// </summary>
    public StatusEnum? Status { get; set; }
}
