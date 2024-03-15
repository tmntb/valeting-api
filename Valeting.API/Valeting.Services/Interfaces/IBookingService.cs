using Valeting.Business.Booking;
using Valeting.Services.Objects.Booking;

namespace Valeting.Services.Interfaces;

public interface IBookingService
{
    Task<CreateBookingSVResponse> CreateAsync(CreateBookingSVRequest createBookingSVRequest);
    Task<UpdateBookingSVResponse> UpdateAsync(UpdateBookingSVRequest updateBookingSVRequest);
    Task<DeleteBookingSVResponse> DeleteAsync(DeleteBookingSVRequest deleteBookingSVRequest);
    Task<GetBookingSVResponse> GetAsync(GetBookingSVRequest getBookingSVRequest);
    Task<PaginatedBookingSVResponse> ListAllAsync(PaginatedBookingSVRequest paginatedBookingSVRequest);
}