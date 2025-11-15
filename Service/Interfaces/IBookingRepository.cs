using Service.Models.Booking;
using Service.Models.Booking.Payload;

namespace Service.Interfaces;

public interface IBookingRepository
{
    /// <summary>
    /// Adds a new booking record to the database.
    /// </summary>
    /// <param name="bookingDto">The booking data to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateAsync(BookingDto bookingDto);

    /// <summary>
    /// Updates an existing booking in the database.
    /// </summary>
    /// <param name="bookingDto">The booking data with updated values.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(BookingDto bookingDto);

    /// <summary>
    /// Deletes a booking from the database by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the booking to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Retrieves a filtered list of bookings from the database.
    /// </summary>
    /// <param name="bookingFilterDto">The filter criteria for retrieving bookings.</param>
    /// <returns>A task that returns a list of <see cref="BookingDto"/> matching the filter criteria.</returns>
    Task<List<BookingDto>> GetFilteredAsync(BookingFilterDto bookingFilterDto);

    /// <summary>
    /// Retrieves a single booking by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the booking to retrieve.</param>
    /// <returns>A task that returns the <see cref="BookingDto"/> if found; otherwise, null.</returns>
    Task<BookingDto> GetByIdAsync(Guid id);
}