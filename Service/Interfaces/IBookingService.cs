using Service.Models.Booking;
using Service.Models.Booking.Payload;

namespace Service.Interfaces;

public interface IBookingService
{
    /// <summary>
    /// Creates a new booking asynchronously.
    /// </summary>
    /// <param name="bookingDto">The booking data transfer object containing the details of the booking to create.</param>
    /// <returns>The unique identifier (<see cref="Guid"/>) of the newly created booking.</returns>
    /// <exception cref="ValidationException">Thrown when the booking data fails validation.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs while creating the booking.</exception>
    Task<Guid> CreateAsync(BookingDto bookingDto);

    /// <summary>
    /// Updates an existing booking asynchronously.
    /// </summary>
    /// <param name="bookingDto">The booking data transfer object containing the updated details of the booking.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ValidationException">Thrown when the booking data fails validation.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no booking is found with the provided ID.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs while updating the booking.</exception>
    Task UpdateAsync(BookingDto bookingDto);

    /// <summary>
    /// Updates the status of an existing booking asynchronously.
    /// </summary>
    /// <param name="updateBookingStatusDtoRequest">The request containing the updated status information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no booking is found with the provided ID.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs while updating the booking status.</exception>
    Task UpdateStatusAsync(UpdateBookingStatusDtoRequest updateBookingStatusDtoRequest);

    /// <summary>
    /// Retrieves a booking by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the booking to retrieve.</param>
    /// <returns>The <see cref="BookingDto"/> representing the booking.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no booking is found with the specified ID.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs while retrieving the booking.</exception>
    Task<BookingDto> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a paginated list of bookings for a specific customer based on the specified filter criteria.
    /// </summary>
    /// <param name="bookingFilterDto">The filter criteria used to query and paginate bookings.</param>
    /// <returns>A <see cref="BookingPaginatedDtoResponse"/> containing a paginated list of bookings.</returns>
    /// <exception cref="ValidationException">Thrown when the provided filter criteria are invalid.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no bookings match the specified filters.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs during retrieval.</exception>
    Task<BookingPaginatedDtoResponse> GetCustomerFilteredAsync(BookingFilterDto bookingFilterDto);

    /// <summary>
    /// Retrieves a paginated list of bookings based on the specified filter criteria.
    /// </summary>
    /// <param name="bookingFilterDto">The filter criteria used to query and paginate bookings.</param>
    /// <returns>A <see cref="BookingPaginatedDtoResponse"/> containing a paginated list of bookings.</returns>
    /// <exception cref="ValidationException">Thrown when the provided filter criteria are invalid.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no bookings match the specified filters.</exception>
    /// <exception cref="Exception">Thrown when an unexpected error occurs during retrieval.</exception>
    Task<BookingPaginatedDtoResponse> GetFilteredAsync(BookingFilterDto bookingFilterDto);
}