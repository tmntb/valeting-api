using Service.Models.Booking;

namespace Repository.Entities;

public partial class Booking
{
    /// <summary>
    /// Creates a <see cref="BookingDto"/> from an <see cref="Booking"/>.
    /// </summary>
    /// <param name="booking">The booking entity.</param>
    /// <returns>A <see cref="BookingDto"/> representing the booking.</returns>
    internal static BookingDto FromEntity(Booking booking)
    {
        return new()
        {
            Id = booking.Id,
            Reference = booking.Reference,
            Customer = ApplicationUser.FromEntity(booking.Customer),
            Flexibility = RdFlexibility.FromEntity(booking.Flexibility),
            VehicleSize = RdVehicleSize.FromEntity(booking.VehicleSize),
            ScheduledAt = booking.ScheduledAt,
            Status = RdStatus.FromEntity(booking.Status),
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt,
            DecisionAt = booking.DecisionAt,
            Decision = booking.DecisionById != null ?ApplicationUser.FromEntity(booking.DecisionBy) : null,
            RequiresApproval = booking.RequiresApproval,
            Notes = booking.Notes
        };
    }
}