using Api.Models.Flexibility;
using Api.Models.Status;
using Api.Models.User;
using Api.Models.VehicleSize;
using Service.Models.Booking;
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
    /// Reference associated with the booking.
    /// </summary>
    public string Reference { get; set; }

    /// <summary>
    /// Customer who made the booking.
    /// </summary>
    public UserApi Customer { get; set; }

    /// <summary>
    /// Flexibility option for the booking.
    /// </summary>
    public FlexibilityApi Flexibility { get; set; }

    /// <summary>
    /// Vehicle size associated with the booking.
    /// </summary>
    public VehicleSizeApi VehicleSize { get; set; }

    /// <summary>
    /// Date and time of the booking.
    /// </summary>
    public DateTime ScheduledAt { get; set; }

    /// <summary>
    /// Current status of the booking.
    /// </summary>
    public StatusApi Status { get; set; }

    /// <summary>
    /// Timestamp when the booking was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the booking was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicates whether the booking has been decided.
    /// </summary>
    public DateTime? DecisionAt { get; set; }

    /// <summary>
    /// Foreign key referencing the user who decided the booking.
    /// </summary>
    public UserApi? DecisionBy { get; set; }

    /// <summary>
    /// Indicates whether the booking requires approval.
    /// </summary>
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// Additional notes or comments related to the booking.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// HATEOAS link for the booking resource.
    /// </summary>
    [JsonPropertyName("_link")]
    public BookingApiLink Link { get; set; }

    internal static BookingApi MapToBookingApi(BookingDto bookingDto, bool includeCustomer = true)
    {
        return new BookingApi
        {
            Id = bookingDto.Id,
            Reference = bookingDto.Reference,
            Customer = includeCustomer ? new()
            {
                Username = bookingDto.Customer.Username,
                ContactNumber = bookingDto.Customer.ContactNumber,
                Email = bookingDto.Customer.Email
            } : null,
            Flexibility = new()
            {
                Name = bookingDto.Flexibility.Name
            },
            VehicleSize = new()
            {
                Name = bookingDto.VehicleSize.Name
            },
            ScheduledAt = bookingDto.ScheduledAt,
            Status = new()
            {
                Name = bookingDto.Status.Name
            },
            CreatedAt = bookingDto.CreatedAt,
            UpdatedAt = bookingDto.UpdatedAt,
            DecisionAt = bookingDto.DecisionAt,
            DecisionBy = bookingDto.Decision != null ? new()
            {
                Username = bookingDto.Decision.Username,
                Email = bookingDto.Decision.Email,
                Role = new()
                {
                    Name = bookingDto.Decision.Role.Name
                }
            } : null,
            RequiresApproval = bookingDto.RequiresApproval,
            Notes = bookingDto.Notes
        };
    }
}