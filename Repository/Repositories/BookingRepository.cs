using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.Booking;
using Service.Models.Booking.Payload;

namespace Repository.Repositories;

public class BookingRepository(ValetingContext valetingContext) : IBookingRepository
{
    /// <inheritdoc />
    public async Task CreateAsync(BookingDto bookingDto)
    {
        var booking = new Booking
        {
            Id = bookingDto.Id,
            Reference = bookingDto.Reference,
            CustomerId = bookingDto.Customer.Id,
            FlexibilityId = bookingDto.Flexibility.Id,
            VehicleSizeId = bookingDto.VehicleSize.Id,
            ScheduledAt = bookingDto.ScheduledAt,
            StatusId = bookingDto.Status.Id,
            CreatedAt = bookingDto.CreatedAt,
            RequiresApproval = bookingDto.RequiresApproval,
            Notes = bookingDto.Notes
        };

        await valetingContext.Bookings.AddAsync(booking);
        await valetingContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(BookingDto bookingDto)
    {
        var booking = await valetingContext.Bookings.FindAsync(bookingDto.Id);
        if (booking == null)
            return;

        booking.FlexibilityId = bookingDto.Flexibility.Id;
        booking.VehicleSizeId = bookingDto.VehicleSize.Id;
        booking.ScheduledAt = bookingDto.ScheduledAt;
        booking.StatusId = bookingDto.Status.Id;
        booking.UpdatedAt = bookingDto.UpdatedAt;
        booking.DecisionAt = bookingDto.DecisionAt;
        booking.DecisionById = bookingDto.Decision?.Id;
        booking.RequiresApproval = bookingDto.RequiresApproval;
        booking.Notes = bookingDto.Notes;

        await valetingContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        var booking = await valetingContext.Bookings.FindAsync(id);
        if (booking == null)
            return;

        valetingContext.Bookings.Remove(booking);
        await valetingContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<BookingDto> GetByIdAsync(Guid id)
    {
        var booking = await valetingContext.Bookings.FindAsync(id);
        if (booking == null)
            return null;

        return new()
        {
            Id = booking.Id,
            Reference = booking.Reference,
            Customer = new()
            {
                Id = booking.Customer.Id,
                Username = booking.Customer.Username,
                Email = booking.Customer.Email,
                Role = new()
                {
                    Id = booking.Customer.Role.Id,
                    Name = booking.Customer.Role.Name
                }
            },
            Flexibility = new()
            {
                Id = booking.Flexibility.Id,
                Name = booking.Flexibility.Name
            },
            VehicleSize = new()
            {
                Id = booking.VehicleSize.Id,
                Name = booking.VehicleSize.Name
            },
            ScheduledAt = booking.ScheduledAt,
            Status = new()
            {
                Id = booking.Status.Id,
                Code = booking.Status.Code,
                Name = booking.Status.Name
            },
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt,
            DecisionAt = booking.DecisionAt,
            Decision = booking.DecisionById != null ? new()
            {
                Id = booking.DecisionBy.Id,
                Username = booking.DecisionBy.Username,
                Email = booking.DecisionBy.Email,
                Role = new()
                {
                    Id = booking.DecisionBy.Role.Id,
                    Name = booking.DecisionBy.Role.Name
                }
            } : null,
            RequiresApproval = booking.RequiresApproval,
            Notes = booking.Notes
        };
    }
    
    /// <inheritdoc />
    public async Task<List<BookingDto>> GetFilteredAsync(BookingFilterDto bookingFilterDto)
    {
        var initialList = await valetingContext.Bookings.ToListAsync();
        var listBookings = from booking in initialList
                            where (bookingFilterDto.CustomerId == null || booking.CustomerId == bookingFilterDto.CustomerId)
                                    && (bookingFilterDto.Status == null || booking.Status.Code == bookingFilterDto.Status)
                           select booking;

        return listBookings.Select(x =>
            new BookingDto
            {
                Id = x.Id,
                Reference = x.Reference,
                Customer = new()
                {
                    Id = x.Customer.Id,
                    Username = x.Customer.Username,
                    Email = x.Customer.Email,
                    Role = new()
                    {
                        Id = x.Customer.Role.Id,
                        Name = x.Customer.Role.Name
                    }
                },
                Flexibility = new()
                {
                    Id = x.Flexibility.Id,
                    Name = x.Flexibility.Name
                },
                VehicleSize = new()
                {
                    Id = x.VehicleSize.Id,
                    Name = x.VehicleSize.Name
                },
                ScheduledAt = x.ScheduledAt,
                Status = new()
                {
                    Id = x.Status.Id,
                    Name = x.Status.Name
                },
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DecisionAt = x.DecisionAt,
                Decision = x.DecisionById != null ? new()
                {
                    Id = x.DecisionBy.Id,
                    Username = x.DecisionBy.Username,
                    Email = x.DecisionBy.Email,
                    Role = new()
                    {
                        Id = x.DecisionBy.Role.Id,
                        Name = x.DecisionBy.Role.Name
                    }
                } : null,
                RequiresApproval = x.RequiresApproval,
                Notes = x.Notes
            }
        ).ToList();
    }
}