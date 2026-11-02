using Service.Models.Booking;
using Service.Models.Booking.Payload;

namespace Service.Interfaces;

public interface IBookingRepository
{
    /// <summary>
    /// Adds a new booking record to the database.
    /// </summary>
    /// <param name="bookingDto">The booking data to create.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateAsync(BookingDto bookingDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing booking in the database.
    /// </summary>
    /// <param name="bookingDto">The booking data with updated values.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(BookingDto bookingDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a booking from the database by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the booking to delete.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single booking by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the booking to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task that returns the <see cref="BookingDto"/> if found; otherwise, null.</returns>
    Task<BookingDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a filtered list of all bookings from the database.
    /// </summary>
    /// <param name="bookingFilterDto">The filter criteria for retrieving bookings.</param>
    /// <param name="cancellationToken">The cancellation token that cancel the operation.</param>
    /// <returns>A task that returns a list of <see cref="BookingDto"/> matching the filter criteria.</returns>
    Task<List<BookingDto>> GetFilteredAsync(BookingFilterDto bookingFilterDto, CancellationToken cancellationToken = default);
}