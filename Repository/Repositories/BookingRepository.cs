using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.Booking;
using Service.Models.Booking.Payload;

namespace Repository.Repositories;

public class BookingRepository(ValetingContext valetingContext) : IBookingRepository
{
    public async Task CreateAsync(BookingDto bookingDto)
    {
        var booking = new Booking 
        {
            Id = bookingDto.Id,
            Name = bookingDto.Name,
            BookingDate = bookingDto.BookingDate,
            ContactNumber = bookingDto.ContactNumber.Value,
            Email = bookingDto.Email,
            Approved = bookingDto.Approved,
            FlexibilityId = bookingDto.Flexibility.Id,
            VehicleSizeId = bookingDto.VehicleSize.Id,
        };

        await valetingContext.Bookings.AddAsync(booking);
        await valetingContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(BookingDto bookingDto)
    {
        var booking = await valetingContext.Bookings.FindAsync(bookingDto.Id);
        if (booking == null)
            return;

        booking.Name = bookingDto.Name;
        booking.BookingDate = bookingDto.BookingDate;
        booking.ContactNumber = bookingDto.ContactNumber.Value;
        booking.Email = bookingDto.Email;
        booking.Approved = bookingDto.Approved;
        booking.FlexibilityId = bookingDto.Flexibility.Id;
        booking.VehicleSizeId = bookingDto.VehicleSize.Id;
        
        await valetingContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var booking = await valetingContext.Bookings.FindAsync(id);
        if (booking == null)
            return;

        valetingContext.Bookings.Remove(booking);
        await valetingContext.SaveChangesAsync();
    }

    public async Task<List<BookingDto>> GetFilteredAsync(BookingFilterDto bookingFilterDto)
    {
        var initialList = await valetingContext.Bookings.ToListAsync();
        var listBookings = from booking in initialList
                            select booking;

        return listBookings.Select(x => 
            new BookingDto
            {
                Id = x.Id,
                Name = x.Name,
                BookingDate = x.BookingDate,
                ContactNumber = x.ContactNumber,
                Email = x.Email,
                Approved = x.Approved,
                Flexibility = new()
                {
                    Id = x.Flexibility.Id,
                    Description = x.Flexibility.Description
                },
                VehicleSize = new()
                {
                    Id = x.VehicleSize.Id,
                    Description= x.VehicleSize.Description
                }
            }
        ).ToList();
    }

    public async Task<BookingDto> GetByIdAsync(Guid id)
    {
        var booking = await valetingContext.Bookings.FindAsync(id);
        if (booking == null)
            return null;

       return new()
       {
           Id = booking.Id,
           Name = booking.Name,
           BookingDate = booking.BookingDate,
           ContactNumber = booking.ContactNumber,
           Email = booking.Email,
           Approved = booking.Approved,
           Flexibility = new()
           {
               Id = booking.Flexibility.Id,
               Description = booking.Flexibility.Description
           },
           VehicleSize = new()
           {
               Id = booking.VehicleSize.Id,
               Description = booking.VehicleSize.Description
           }
       };
    }
}