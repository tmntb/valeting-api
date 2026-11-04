using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Service.Interfaces;
using Service.Models.Booking;
using Service.Models.Booking.Payload;

namespace Repository.Repositories;

public class BookingRepository(ValetingContext valetingContext) : IBookingRepository
{
    /// <inheritdoc />
    public async Task CreateAsync(BookingDto bookingDto, CancellationToken cancellationToken = default)
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

        await valetingContext.Bookings.AddAsync(booking, cancellationToken);
        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(BookingDto bookingDto, CancellationToken cancellationToken = default)
    {
        var booking = await valetingContext.Bookings.FindAsync(bookingDto.Id, cancellationToken);
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

        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var booking = await valetingContext.Bookings.FindAsync(id, cancellationToken);
        if (booking == null)
            return;

        valetingContext.Bookings.Remove(booking);
        await valetingContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BookingDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var booking = await valetingContext.Bookings
            .AsNoTracking()
            .Include(b => b.Customer).ThenInclude(c => c.Role)
            .Include(b => b.Flexibility)
            .Include(b => b.VehicleSize)
            .Include(b => b.Status)
            .Include(b => b.DecisionBy).ThenInclude(d => d.Role)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        
        if (booking == null)
            return null;

        return Booking.FromEntity(booking);
    }
    
    /// <inheritdoc />
    public async Task<List<BookingDto>> GetFilteredAsync(BookingFilterDto bookingFilterDto, CancellationToken cancellationToken = default)
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

        var bookings = await query.ToListAsync(cancellationToken);
        return bookings.Select(Booking.FromEntity).ToList();
    }
}