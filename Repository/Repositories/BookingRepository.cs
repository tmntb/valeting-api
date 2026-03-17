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
        var booking = await valetingContext.Bookings
            .AsNoTracking()
            .Include(b => b.Customer).ThenInclude(c => c.Role)
            .Include(b => b.Flexibility)
            .Include(b => b.VehicleSize)
            .Include(b => b.Status)
            .Include(b => b.DecisionBy).ThenInclude(d => d.Role)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (booking == null)
            return null;

        return FromEntity(booking);
    }
    
    /// <inheritdoc />
    public async Task<List<BookingDto>> GetFilteredAsync(BookingFilterDto bookingFilterDto)
    {
        var query = valetingContext.Bookings
            .AsNoTracking()
            .Include(b => b.Customer).ThenInclude(c => c.Role)
            .Include(b => b.Flexibility)
            .Include(b => b.VehicleSize)
            .Include(b => b.Status)
            .Include(b => b.DecisionBy).ThenInclude(d => d.Role)
            .AsQueryable();

        if (bookingFilterDto.CustomerId.HasValue)
            query = query.Where(b => b.CustomerId == bookingFilterDto.CustomerId.Value);

        if (bookingFilterDto.Status.HasValue)
            query = query.Where(b => b.Status.Code == bookingFilterDto.Status.Value);

        var bookings = await query.ToListAsync();
        return bookings.Select(FromEntity).ToList();
    }

    /// <summary>
    /// Maps a Booking entity to a BookingDto.
    /// </summary>
    /// <param name="booking"></param>
    /// <returns></returns>
    private BookingDto FromEntity(Booking booking)
    {
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
                Name = booking.Flexibility.Name,
                NumberOfMinutes = booking.Flexibility.NumberOfMinutes
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
}