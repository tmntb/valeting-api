using Microsoft.EntityFrameworkCore;

using Valeting.Business.Booking;
using Valeting.Business.Flexibility;
using Valeting.Business.VehicleSize;
using Valeting.Repositories.Entities;
using Valeting.Repositories.Interfaces;

namespace Valeting.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ValetingContext _valetingContext;

        public BookingRepository(ValetingContext valetingContext)
        {
            this._valetingContext = valetingContext;
        }

        public async Task CreateAsync(BookingDTO bookingDTO)
        {
            var booking = new Booking()
            {
                Id = bookingDTO.Id,
                Name = bookingDTO.Name,
                BookingDate = bookingDTO.BookingDate,
                FlexibilityId = bookingDTO.Flexibility.Id,
                VehicleSizeId = bookingDTO.VehicleSize.Id,
                ContactNumber = bookingDTO.ContactNumber.Value,
                Email = bookingDTO.Email,
            };

            await _valetingContext.Bookings.AddAsync(booking);
            await _valetingContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(BookingDTO bookingDTO)
        {
            var booking = await _valetingContext.Bookings.FindAsync(bookingDTO.Id);
            if (booking == null)
                return;

            booking.Name = bookingDTO.Name;
            booking.BookingDate = bookingDTO.BookingDate;
            booking.FlexibilityId = bookingDTO.Flexibility.Id;
            booking.VehicleSizeId = bookingDTO.VehicleSize.Id;
            booking.ContactNumber = bookingDTO.ContactNumber.Value;
            booking.Email = bookingDTO.Email;
            booking.Approved = bookingDTO.Approved;

            _valetingContext.Bookings.Update(booking);
            await _valetingContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var booking = await _valetingContext.Bookings.FindAsync(id);
            if (booking == null)
                return;

            _valetingContext.Bookings.Remove(booking);
            await _valetingContext.SaveChangesAsync();
        }

        public async Task<BookingDTO> FindByIdAsync(Guid id)
        {
            var booking = await _valetingContext.Bookings.FindAsync(id);
            if (booking == null)
                return null;

            var bookingDTO = new BookingDTO()
            {
                Id = id,
                Name = booking.Name,
                BookingDate = booking.BookingDate,
                Flexibility = new FlexibilityDTO() { Id = booking.Flexibility.Id, Description = booking.Flexibility.Description, Active = booking.Flexibility.Active },
                VehicleSize = new VehicleSizeDTO() { Id = booking.VehicleSize.Id, Description = booking.VehicleSize.Description, Active = booking.VehicleSize.Active },
                ContactNumber = booking.ContactNumber,
                Email = booking.Email,
                Approved = booking.Approved
            };

            return bookingDTO;
        }

        public async Task<BookingListDTO> ListAsync(BookingFilterDTO bookingFilterDTO)
        {
            var bookingListDTO = new BookingListDTO() { Bookings = new List<BookingDTO>() };

            var initialList = await _valetingContext.Bookings.ToListAsync();
            var listBookings = from booking in initialList
                               select booking;

            if (listBookings == null)
                return bookingListDTO;

            bookingListDTO.TotalItems = listBookings.Count();
            var nrPages = Decimal.Divide(bookingListDTO.TotalItems, bookingFilterDTO.PageSize);
            var nrPagesTruncate = Math.Truncate(nrPages);
            bookingListDTO.TotalPages = (int)(nrPages - nrPagesTruncate > 0 ? nrPagesTruncate + 1 : nrPagesTruncate);

            listBookings = listBookings.OrderBy(x => x.Id);

            listBookings = listBookings.Skip((bookingFilterDTO.PageNumber - 1) * bookingFilterDTO.PageSize).Take(bookingFilterDTO.PageSize);

            bookingListDTO.Bookings.AddRange(
                listBookings.ToList().Select(item => new BookingDTO()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        BookingDate = item.BookingDate,
                        Flexibility = new FlexibilityDTO() { Id = item.Flexibility.Id, Description = item.Flexibility.Description, Active = item.Flexibility.Active },
                        VehicleSize = new VehicleSizeDTO() { Id = item.VehicleSize.Id, Description = item.VehicleSize.Description, Active = item.VehicleSize.Active },
                        ContactNumber = item.ContactNumber,
                        Email = item.Email,
                        Approved = item.Approved
                    }
                ).ToList()
            );

            return bookingListDTO;
        }
    }
}
