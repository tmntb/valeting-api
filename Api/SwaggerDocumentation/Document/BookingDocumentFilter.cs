using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

/// <summary>
/// Custom Swagger/OpenAPI document filter for the Booking endpoints.
/// </summary>
/// <remarks>
/// This filter adds descriptive metadata for all booking-related operations,
/// including listing, creating, retrieving by id, updating, and deleting bookings.
/// It sets operation IDs, summaries, and descriptions for better API documentation.
/// </remarks>
public class BookingDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint for the general bookings collection.
    /// </summary>
    public const string BookingsEndpoint = "/bookings";

    /// <summary>
    /// Endpoint for a single booking by its identifier.
    /// </summary>
    public const string BookingsIdEndpoint = "/bookings/{id}";

    /// <summary>
    /// Applies the filter to the given OpenAPI document.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document to modify.</param>
    /// <param name="context">The document filter context.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "Booking", Description = "Booking operations" });

        var bookingsPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == BookingsEndpoint).Value;
        bookingsPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.OperationId = "get-bookings";
        bookingsPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.Summary = "List Bookings";
        bookingsPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.Description = "Return a list of all **Bookings**, it can be filtered by the page number and page size";

        bookingsPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.OperationId = "post-bookings";
        bookingsPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Summary = "Create booking";
        bookingsPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Post).Value.Description = "Creates a new **Booking**";

        var bookingsIdPaths = swaggerDoc.Paths.FirstOrDefault(x => x.Key == BookingsIdEndpoint).Value;
        bookingsIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.OperationId = "get-bookings-id";
        bookingsIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.Summary = "List booking by id";
        bookingsIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Get).Value.Description = "Return a **Booking** by the given id";

        bookingsIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Put).Value.OperationId = "put-bookings-id";
        bookingsIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Put).Value.Summary = "Update booking";
        bookingsIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Put).Value.Description = "Update **Booking** by the given id";
        
        bookingsIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Delete).Value.OperationId = "delete-bookings-id";
        bookingsIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Delete).Value.Summary = "Delete booking";
        bookingsIdPaths.Operations.FirstOrDefault(x => x.Key == OperationType.Delete).Value.Description = "Delete **Booking** by the given id";
    }
}