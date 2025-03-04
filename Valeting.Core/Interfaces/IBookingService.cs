using Valeting.Common.Models.Booking;

namespace Valeting.Core.Interfaces;

public interface IBookingService
{
    Task<CreateBookingDtoResponse> CreateAsync(CreateBookingDtoRequest createBookingDtoRequest);
    Task<UpdateBookingDtoResponse> UpdateAsync(UpdateBookingDtoRequest updateBookingDtoRequest);
    Task<DeleteBookingDtoResponse> DeleteAsync(DeleteBookingDtoRequest deleteBookingDtoRequest);
    Task<GetBookingDtoResponse> GetByIdAsync(GetBookingDtoRequest getBookingDtoRequest);
    Task<PaginatedBookingDtoResponse> GetAsync(PaginatedBookingDtoRequest paginatedBookingDtoRequest);
}