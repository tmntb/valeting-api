using Service.Models.Booking;
using Service.Models.Booking.Payload;

namespace Service.Interfaces;

public interface IBookingService
{
    /// <summary>
    /// Creates a new booking asynchronously.
    /// </summary>
    /// <remarks>
    /// This method validates the provided <see cref="BookingDto"/> using the <see cref="CreateBookingValidator"/>.  
    /// If the data is valid, a new unique identifier is generated and assigned to the booking.  
    /// The booking is then stored in the database via the repository, and the relevant cache entries are invalidated
    /// to ensure data consistency.
    /// </remarks>
    /// <param name="bookingDto">The booking data transfer object containing the details of the booking to create.</param>
    /// <returns>The unique identifier (<see cref="Guid"/>) of the newly created booking.</returns>
    /// <exception cref="ValidationException">Thrown when the booking data fails validation.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs while creating the booking.</exception>
    Task<Guid> CreateAsync(BookingDto bookingDto);

    /// <summary>
    /// Updates an existing booking asynchronously.
    /// </summary>
    /// <remarks>
    /// This method validates the provided <see cref="BookingDto"/> using the <see cref="UpdateBookingValidator"/>.  
    /// It retrieves the existing booking from the repository using the provided ID and updates its fields with the new values.  
    /// Once updated, the booking is persisted back to the database, and the cache is invalidated to maintain consistency.
    /// </remarks>
    /// <param name="bookingDto">The booking data transfer object containing the updated details of the booking.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ValidationException">Thrown when the booking data fails validation.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no booking is found with the provided ID.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs while updating the booking.</exception>
    Task UpdateAsync(BookingDto bookingDto);

    /// <summary>
    /// Deletes an existing booking asynchronously.
    /// </summary>
    /// <remarks>
    /// This method retrieves the booking by its unique identifier and removes it from the repository.  
    /// After successful deletion, related cache entries are invalidated to ensure data consistency.
    /// </remarks>
    /// <param name="id">The unique identifier of the booking to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no booking is found with the provided ID.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs while deleting the booking.</exception>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Retrieves a booking by its unique identifier asynchronously.
    /// </summary>
    /// <remarks>
    /// This method attempts to fetch the booking from the cache first.  
    /// If not found, it retrieves the booking from the repository, stores it in the cache and returns the result. Cached entries expire after one day.
    /// </remarks>
    /// <param name="id">The unique identifier of the booking to retrieve.</param>
    /// <returns>The <see cref="BookingDto"/> representing the booking.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no booking is found with the specified ID.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs while retrieving the booking.</exception>
    Task<BookingDto> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a paginated list of bookings based on the specified filter criteria.
    /// </summary>
    /// <remarks>
    /// This method first validates the <see cref="BookingFilterDto"/> request then checks the cache for a matching paginated result.  
    /// If not found, it fetches the filtered data from the repository, applies pagination stores the result in cache for 5 minutes, and returns it.  
    /// </remarks>
    /// <param name="bookingFilterDto">The filter criteria used to query and paginate bookings.</param>
    /// <returns>A <see cref="BookingPaginatedDtoResponse"/> containing a paginated list of bookings.</returns>
    /// <exception cref="ValidationException">Thrown when the provided filter criteria are invalid.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no bookings match the specified filters.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs during retrieval or caching.</exception>
    Task<BookingPaginatedDtoResponse> GetFilteredAsync(BookingFilterDto bookingFilterDto);
}