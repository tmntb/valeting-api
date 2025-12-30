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
            ScheduledAt = bookingDto.ScheduledAt,
            RequiresApproval = bookingDto.RequiresApproval,
            FlexibilityId = bookingDto.Flexibility.Id,
            VehicleSizeId = bookingDto.VehicleSize.Id,
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

        booking.Reference = bookingDto.Reference;
        booking.ScheduledAt = bookingDto.ScheduledAt;
        booking.RequiresApproval = bookingDto.RequiresApproval;
        booking.FlexibilityId = bookingDto.Flexibility.Id;
        booking.VehicleSizeId = bookingDto.VehicleSize.Id;
        
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
    public async Task<List<BookingDto>> GetFilteredAsync(BookingFilterDto bookingFilterDto)
    {
        var initialList = await valetingContext.Bookings.ToListAsync();
        var listBookings = from booking in initialList
                            select booking;

        return listBookings.Select(x => 
            new BookingDto
            {
                Id = x.Id,
                Reference = x.Reference,
                ScheduledAt = x.ScheduledAt,
                RequiresApproval = x.RequiresApproval,
                Flexibility = new()
                {
                    Id = x.Flexibility.Id,
                    Name = x.Flexibility.Name
                },
                VehicleSize = new()
                {
                    Id = x.VehicleSize.Id,
                    Name= x.VehicleSize.Name
                }
            }
        ).ToList();
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
           ScheduledAt = booking.ScheduledAt,
           RequiresApproval = booking.RequiresApproval,
           Flexibility = new()
           {
               Id = booking.Flexibility.Id,
               Name = booking.Flexibility.Name
           },
           VehicleSize = new()
           {
               Id = booking.VehicleSize.Id,
               Name = booking.VehicleSize.Name
           }
       };
    }
}