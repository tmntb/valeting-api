using Api.Models.Core;

namespace Api.Models.Booking;

/// <summary>
/// Represents HATEOAS links for a booking resource.
/// </summary>
public class BookingApiLink
{
    /// <summary>
    /// Self-link of the booking resource.
    /// </summary>
    public LinkApi Self { get; set; }
}
