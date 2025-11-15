using Api.Models.Core;

namespace Api.Models.Booking.Payload;

/// <summary>
/// Represents the query string parameters used to filter and paginate booking results.
/// Inherits common query parameters such as <c>PageNumber</c> and <c>PageSize</c> from <see cref="QueryStringParametersApi"/>.
/// </summary>
public class BookingApiParameters : QueryStringParametersApi { }
