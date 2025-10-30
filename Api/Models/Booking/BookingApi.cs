using Api.Models.Flexibility;
using Api.Models.VehicleSize;
using System.Text.Json.Serialization;

namespace Api.Models.Booking;

/// <summary>
/// Represents a booking with all its details for API responses.
/// </summary>
public class BookingApi
{
    /// <summary>
    /// Unique identifier of the booking.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name associated with the booking.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Date and time of the booking.
    /// </summary>
    public DateTime BookingDate { get; set; }

    /// <summary>
    /// Flexibility option for the booking.
    /// </summary>
    public FlexibilityApi Flexibility { get; set; }

    /// <summary>
    /// Vehicle size associated with the booking.
    /// </summary>
    public VehicleSizeApi VehicleSize { get; set; }

    /// <summary>
    /// Contact number provided for the booking.
    /// </summary>
    public int ContactNumber { get; set; }

    /// <summary>
    /// Email address provided for the booking.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Indicating whether the booking has been approved.
    /// </summary>
    public bool? Approved { get; set; }

    /// <summary>
    /// HATEOAS link for the booking resource.
    /// </summary>
    [JsonPropertyName("_link")]
    public BookingApiLink Link { get; set; }
}