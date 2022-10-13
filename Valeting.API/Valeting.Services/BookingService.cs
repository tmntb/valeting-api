using Valeting.Business;
using Valeting.Services.Interfaces;
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

            bookingDTO.Id = Guid.NewGuid();

            await _bookingRepository.CreateAsync(bookingDTO);
            return bookingDTO;
        }

        public async Task UpdateAsync(BookingDTO bookingDTO)
        {
            ValidateGeneralInput(bookingDTO);

            ValidateBookingId(bookingDTO.Id);

            await _bookingRepository.UpdateAsync(bookingDTO);
        }

        public async Task DeleteAsync(Guid id)
        {
            ValidateBookingId(id);

            await _bookingRepository.DeleteAsync(id);
        }

        public async Task<BookingDTO> FindByIDAsync(Guid id)
        {
            ValidateBookingId(id);

            var bookingDTO = await _bookingRepository.FindByIDAsync(id);
            if (bookingDTO == null)
                throw new Exception("Booking not found");

            return bookingDTO;
        }

        public async Task<IEnumerable<BookingDTO>> ListAllAsync()
        {
            return await _bookingRepository.ListAsync();
        }

        private void ValidateGeneralInput(BookingDTO bookingDTO)
        {
            if (bookingDTO == null)
                throw new Exception("bookingDTO is null");

            if (bookingDTO.Flexibility == null || bookingDTO.Flexibility.Id.Equals(Guid.Empty))
            {
                string errorMsg = bookingDTO.Flexibility == null ? "Flexibility is null" : "FlexibilityId is empty";
                throw new Exception(errorMsg);
            }

            if (bookingDTO.VehicleSize == null || bookingDTO.VehicleSize.Id.Equals(Guid.Empty))
            {
                string errorMsg = bookingDTO.Flexibility == null ? "VehicleSize is null" : "VehicleSizeId is empty";
                throw new Exception(errorMsg);
            }

            if (string.IsNullOrEmpty(bookingDTO.Name) || string.IsNullOrEmpty(bookingDTO.Email))
            {
                string errorMsg = string.IsNullOrEmpty(bookingDTO.Name) ? "Name is null or empty" : "Email is null or empty";
                throw new Exception(errorMsg);
            }
        }

        private void ValidateBookingId(Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new Exception("BookingId is empty");
        }
    }
}