using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Valeting.Repository.Entities;
using Valeting.Repository.Models.Booking;
using Valeting.Repository.Repositories.Interfaces;

namespace Valeting.Repository.Repositories;

public class BookingRepository(ValetingContext valetingContext, IMapper mapper) : IBookingRepository
{
    public async Task CreateAsync(BookingDTO bookingDTO)
    {
        var booking = mapper.Map<Booking>(bookingDTO);
        await valetingContext.Bookings.AddAsync(booking);
        await valetingContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(BookingDTO bookingDTO)
    {
        var bookingCheck = await valetingContext.Bookings.FindAsync(bookingDTO.Id);
        if (bookingCheck == null)
            return;

        mapper.Map(bookingDTO, bookingCheck);
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

    public async Task<BookingDTO> GetByIDAsync(Guid id)
    {
        var booking = await valetingContext.Bookings.FindAsync(id);
        if (booking == null)
            return null;

       return mapper.Map<BookingDTO>(booking);
    }

    public async Task<BookingListDTO> GetAsync(BookingFilterDTO bookingFilterDTO)
    {
        var bookingListDTO = new BookingListDTO();

        var initialList = await valetingContext.Bookings.ToListAsync();
        var listBookings = from booking in initialList
                            select booking;

        if (listBookings == null)
            return bookingListDTO;

        bookingListDTO.TotalItems = listBookings.Count();
        var nrPages = decimal.Divide(bookingListDTO.TotalItems, bookingFilterDTO.PageSize);
        var nrPagesTruncate = Math.Truncate(nrPages);
        bookingListDTO.TotalPages = (int)(nrPages - nrPagesTruncate > 0 ? nrPagesTruncate + 1 : nrPagesTruncate);

        listBookings = listBookings.OrderBy(x => x.Id);
        listBookings = listBookings.Skip((bookingFilterDTO.PageNumber - 1) * bookingFilterDTO.PageSize).Take(bookingFilterDTO.PageSize);
        bookingListDTO.Bookings = mapper.Map<List<BookingDTO>>(listBookings);
        return bookingListDTO;
    }
}