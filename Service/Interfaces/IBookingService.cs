using Service.Models.Booking;
using Service.Models.Booking.Payload;

namespace Service.Interfaces;

public interface IBookingService
{
    /// <summary>
    /// Creates a new booking asynchronously.
    /// </summary>
    /// <param name="bookingDto">The booking data transfer object containing the details of the booking to create.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>The unique identifier (<see cref="Guid"/>) of the newly created booking.</returns>
    Task<Guid> CreateAsync(BookingDto bookingDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing booking asynchronously.
    /// </summary>
    /// <param name="bookingDto">The booking data transfer object containing the updated details of the booking.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(BookingDto bookingDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the status of an existing booking asynchronously.
    /// </summary>
    /// <param name="updateBookingStatusDtoRequest">The request containing the updated status information.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateStatusAsync(UpdateBookingStatusDtoRequest updateBookingStatusDtoRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a booking by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the booking to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>The <see cref="BookingDto"/> representing the booking.</returns>
    Task<BookingDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of bookings for a specific customer based on the specified filter criteria.
    /// </summary>
    /// <param name="bookingFilterDto">The filter criteria used to query and paginate bookings.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A <see cref="BookingPaginatedDtoResponse"/> containing a paginated list of bookings.</returns>
    Task<BookingPaginatedDtoResponse> GetCustomerFilteredAsync(BookingFilterDto bookingFilterDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of bookings based on the specified filter criteria.
    /// </summary>
    /// <param name="bookingFilterDto">The filter criteria used to query and paginate bookings.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A <see cref="BookingPaginatedDtoResponse"/> containing a paginated list of bookings.</returns>
    Task<BookingPaginatedDtoResponse> GetFilteredAsync(BookingFilterDto bookingFilterDto, CancellationToken cancellationToken = default);
}