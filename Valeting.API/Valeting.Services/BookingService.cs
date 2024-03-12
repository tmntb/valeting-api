using Valeting.Common.Messages;
using Valeting.Business.Booking;
using Valeting.Common.Exceptions;
using Valeting.Services.Interfaces;
using Valeting.Repositories.Interfaces;

namespace Valeting.Service
{
    public class BookingService(IBookingRepository bookingRepository) : IBookingService
    {
        public async Task<BookingDTO> CreateAsync(BookingDTO bookingDTO)
        {
            ValidateGeneralInput(bookingDTO);

            if (bookingDTO.BookingDate < DateTime.Now)
                throw new BusinessObjectException(Messages.DateInThePast);

            bookingDTO.Id = Guid.NewGuid();

            await bookingRepository.CreateAsync(bookingDTO);
            return bookingDTO;
        }

        public async Task UpdateAsync(BookingDTO bookingDTO)
        {
            ValidateGeneralInput(bookingDTO);

            ValidateBookingId(bookingDTO.Id);

            if (!bookingDTO.Approved.HasValue)
                throw new InputException(Messages.ApprovedEmpty);

            if (bookingDTO.BookingDate < DateTime.Now)
                throw new BusinessObjectException(Messages.DateInThePast);

            BookingDTO bookingDTO_Check = await bookingRepository.FindByIdAsync(bookingDTO.Id);
            if (bookingDTO_Check == null)
                throw new NotFoundException(Messages.BookingNotFound);

            await bookingRepository.UpdateAsync(bookingDTO);
        }

        public async Task DeleteAsync(Guid id)
        {
            ValidateBookingId(id);

            BookingDTO bookingDTO_Check = await bookingRepository.FindByIdAsync(id);
            if (bookingDTO_Check == null)
                throw new NotFoundException(Messages.BookingNotFound);

            await bookingRepository.DeleteAsync(id);
        }

        public async Task<BookingDTO> FindByIDAsync(Guid id)
        {
            ValidateBookingId(id);

            var bookingDTO = await bookingRepository.FindByIdAsync(id);
            if (bookingDTO == null)
                throw new NotFoundException(Messages.BookingNotFound);

            return bookingDTO;
        }

        public async Task<BookingListDTO> ListAllAsync(BookingFilterDTO bookingFilterDTO)
        {
            if (bookingFilterDTO.PageNumber == 0)
                throw new InputException(Messages.InvalidPageNumber);

            return await bookingRepository.ListAsync(bookingFilterDTO);
        }

        private void ValidateGeneralInput(BookingDTO bookingDTO)
        {
            if (bookingDTO == null)
                throw new BusinessObjectException(Messages.BookingDTONotPopulated);

            if (bookingDTO.Flexibility == null || bookingDTO.Flexibility.Id.Equals(Guid.Empty))
            {
                string errorMsg = bookingDTO.Flexibility == null ? Messages.FlexibilityNull : Messages.FlexibilityEmpty;
                throw new InputException(errorMsg);
            }

            if (bookingDTO.VehicleSize == null || bookingDTO.VehicleSize.Id.Equals(Guid.Empty))
            {
                string errorMsg = bookingDTO.Flexibility == null ? Messages.VehicleSizeNull : Messages.VehicleSizeEmpty;
                throw new InputException(errorMsg);
            }

            if (string.IsNullOrEmpty(bookingDTO.Name) || string.IsNullOrEmpty(bookingDTO.Email) || !bookingDTO.ContactNumber.HasValue || bookingDTO.BookingDate.Equals(DateTime.MinValue))
            {
                string errorMsg = string.IsNullOrEmpty(bookingDTO.Name) ? Messages.InvalidBookingName :
                                string.IsNullOrEmpty(bookingDTO.Email) ? Messages.InvalidBookingEmail :
                                !bookingDTO.ContactNumber.HasValue ? Messages.InvalidBookingContactNumber : Messages.InvalidBookingDate;
                throw new InputException(errorMsg);
            }
        }

        private void ValidateBookingId(Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new InputException(Messages.InvalidBookingId);
        }
    }
}