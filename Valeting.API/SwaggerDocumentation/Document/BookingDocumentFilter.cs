using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Valeting.API.SwaggerDocumentation.Document;

public class BookingDocumentFilter : IDocumentFilter
{
    public const string BookingsEndpoint = "/bookings";
    public const string BookingsIdEndpoint = "/bookings/{id}";

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