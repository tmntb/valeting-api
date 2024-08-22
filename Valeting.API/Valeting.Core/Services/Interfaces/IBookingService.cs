using Valeting.Core.Models.Booking;

namespace Valeting.Core.Services.Interfaces;

public interface IBookingService
{
    Task<CreateBookingSVResponse> CreateAsync(CreateBookingSVRequest createBookingSVRequest);
    Task<UpdateBookingSVResponse> UpdateAsync(UpdateBookingSVRequest updateBookingSVRequest);
    Task<DeleteBookingSVResponse> DeleteAsync(DeleteBookingSVRequest deleteBookingSVRequest);
    Task<GetBookingSVResponse> GetByIdAsync(GetBookingSVRequest getBookingSVRequest);
    Task<PaginatedBookingSVResponse> GetAsync(PaginatedBookingSVRequest paginatedBookingSVRequest);
}