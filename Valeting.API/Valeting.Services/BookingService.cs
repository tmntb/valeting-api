using Valeting.Business.Booking;
using Valeting.Common.Exceptions;
using Valeting.Services.Interfaces;
using Valeting.Business.VehicleSize;
using Valeting.Repositories.Interfaces;

namespace Valeting.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            this._bookingRepository = bookingRepository ?? throw new Exception("bookingRepository is null");
        }

        public async Task<BookingDTO> CreateAsync(BookingDTO bookingDTO)
        {
            ValidateGeneralInput(bookingDTO);

            if (bookingDTO.BookingDate < DateTime.Now)
                throw new BusinessObjectException("Booking date cannot be in the past");

            bookingDTO.Id = Guid.NewGuid();

            await _bookingRepository.CreateAsync(bookingDTO);
            return bookingDTO;
        }

        public async Task UpdateAsync(BookingDTO bookingDTO)
        {
            ValidateGeneralInput(bookingDTO);

            ValidateBookingId(bookingDTO.Id);

            if (!bookingDTO.Approved.HasValue)
                throw new InputException("Approved is null or empty");

            if (bookingDTO.BookingDate < DateTime.Now)
                throw new BusinessObjectException("Booking date cannot be in the past");

            BookingDTO bookingDTO_Check = await _bookingRepository.FindByIdAsync(bookingDTO.Id);
            if (bookingDTO_Check == null)
                throw new NotFoundException("Not found booking");

            await _bookingRepository.UpdateAsync(bookingDTO);
        }

        public async Task DeleteAsync(Guid id)
        {
            ValidateBookingId(id);

            BookingDTO bookingDTO_Check = await _bookingRepository.FindByIdAsync(id);
            if (bookingDTO_Check == null)
                throw new NotFoundException("Not found booking");

            await _bookingRepository.DeleteAsync(id);
        }

        public async Task<BookingDTO> FindByIDAsync(Guid id)
        {
            ValidateBookingId(id);

            var bookingDTO = await _bookingRepository.FindByIdAsync(id);
            if (bookingDTO == null)
                throw new NotFoundException("Booking not found");

            return bookingDTO;
        }

        public async Task<BookingListDTO> ListAllAsync(BookingFilterDTO bookingFilterDTO)
        {
            if (bookingFilterDTO.PageNumber == 0)
                throw new InputException("pageNumber é 0");

            return await _bookingRepository.ListAsync(bookingFilterDTO);
        }

        private void ValidateGeneralInput(BookingDTO bookingDTO)
        {
            if (bookingDTO == null)
                throw new BusinessObjectException("bookingDTO is null");

            if (bookingDTO.Flexibility == null || bookingDTO.Flexibility.Id.Equals(Guid.Empty))
            {
                string errorMsg = bookingDTO.Flexibility == null ? "Flexibility is null" : "FlexibilityId is empty";
                throw new InputException(errorMsg);
            }

            if (bookingDTO.VehicleSize == null || bookingDTO.VehicleSize.Id.Equals(Guid.Empty))
            {
                string errorMsg = bookingDTO.Flexibility == null ? "VehicleSize is null" : "VehicleSizeId is empty";
                throw new InputException(errorMsg);
            }

            if (string.IsNullOrEmpty(bookingDTO.Name) || string.IsNullOrEmpty(bookingDTO.Email) || !bookingDTO.ContactNumber.HasValue || bookingDTO.BookingDate.Equals(DateTime.MinValue))
            {
                string errorMsg = string.IsNullOrEmpty(bookingDTO.Name) ? "Name is null or empty" :
                                string.IsNullOrEmpty(bookingDTO.Email) ? "Email is null or empty" :
                                !bookingDTO.ContactNumber.HasValue ? "Contact Number is null or empty" : "Booking date is null or empty";
                throw new InputException(errorMsg);
            }
        }

        private void ValidateBookingId(Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new InputException("BookingId is empty");
        }
    }
}