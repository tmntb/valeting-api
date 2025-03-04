using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Valeting.Repository.Entities;
using Valeting.Repository.Interfaces;
using Valeting.Common.Models.Booking;

namespace Valeting.Repository.Repositories;

public class BookingRepository(ValetingContext valetingContext, IMapper mapper) : IBookingRepository
{
    public async Task CreateAsync(BookingDto bookingDto)
    {
        var booking = mapper.Map<Booking>(bookingDto);
        await valetingContext.Bookings.AddAsync(booking);
        await valetingContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(BookingDto bookingDto)
    {
        var bookingCheck = await valetingContext.Bookings.FindAsync(bookingDto.Id);
        if (bookingCheck == null)
            return;

        mapper.Map(bookingDto, bookingCheck);
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

    public async Task<BookingListDto> GetAsync(BookingFilterDto bookingFilterDto)
    {
        var bookingListDto = new BookingListDto() { Bookings = [] };

        var initialList = await valetingContext.Bookings.ToListAsync();
        var listBookings = from booking in initialList
                            select booking;

        if (listBookings == null)
            return bookingListDto;

        bookingListDto.TotalItems = listBookings.Count();
        var nrPages = decimal.Divide(bookingListDto.TotalItems, bookingFilterDto.PageSize);
        var nrPagesTruncate = Math.Truncate(nrPages);
        bookingListDto.TotalPages = (int)(nrPages - nrPagesTruncate > 0 ? nrPagesTruncate + 1 : nrPagesTruncate);

        listBookings = listBookings.OrderBy(x => x.Id);
        listBookings = listBookings.Skip((bookingFilterDto.PageNumber - 1) * bookingFilterDto.PageSize).Take(bookingFilterDto.PageSize);
        bookingListDto.Bookings = mapper.Map<List<BookingDto>>(listBookings);
        return bookingListDto;
    }

    public async Task<BookingDto> GetByIdAsync(Guid id)
    {
        var booking = await valetingContext.Bookings.FindAsync(id);
        if (booking == null)
            return null;

       return mapper.Map<BookingDto>(booking);
    }
}