using Common.Models.Booking;

namespace Service.Interfaces;

public interface IBookingService
{
    Task<CreateBookingDtoResponse> CreateAsync(CreateBookingDtoRequest createBookingDtoRequest);
    Task UpdateAsync(UpdateBookingDtoRequest updateBookingDtoRequest);
    Task DeleteAsync(DeleteBookingDtoRequest deleteBookingDtoRequest);
    Task<GetBookingDtoResponse> GetByIdAsync(GetBookingDtoRequest getBookingDtoRequest);
    Task<PaginatedBookingDtoResponse> GetFilteredAsync(PaginatedBookingDtoRequest paginatedBookingDtoRequest);
}