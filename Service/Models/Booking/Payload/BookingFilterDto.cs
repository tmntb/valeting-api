using Service.Models.Core;

namespace Service.Models.Booking.Payload;

/// <summary>
/// Represents the filter parameters used when querying bookings with pagination support.
/// Inherits common filtering properties from <see cref="FilterDto"/> such as PageNumber and PageSize.
/// </summary>
public class BookingFilterDto : FilterDto { }
