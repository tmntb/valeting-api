using System.Diagnostics.CodeAnalysis;
using System.Net;
using Api.Extensions;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.SwaggerDocumentation.Document;

/// <summary>
/// Custom Swagger/OpenAPI document filter for the Booking endpoints.
/// </summary>
[ExcludeFromCodeCoverage]
public class BookingDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Endpoint for the general bookings collection.
    /// </summary>
    private const string BookingEndpoint = "/api/v1/booking";

    /// <summary>
    /// Endpoint for a single booking by its identifier.
    /// </summary>
    private const string BookingIdEndpoint = "/api/v1/booking/{id}";

    /// <summary>
    /// Endpoint for updating the status of a booking by its identifier.
    /// </summary>
    private const string BookingIdStatusEndpoint = "/api/v1/booking/{id}/status";

    /// <summary>
    /// Endpoint for retrieving bookings filtered by customer.
    /// </summary>
    private const string BookingCustomerEndpoint = "/api/v1/booking/customer";

    /// <summary>
    /// Applies the filter to the given OpenAPI document.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document to modify.</param>
    /// <param name="context">The document filter context.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags.Add(new OpenApiTag() { Name = "Booking", Description = "Booking operations" });
        
        swaggerDoc.ConfigureOperation(BookingEndpoint, HttpMethod.Get, "get-Bookings",
            "List Booking", "Return a list of all **Bookings**, it can be filtered by the page number and page size");
        
        swaggerDoc.ConfigureOperation(BookingEndpoint, HttpMethod.Post, "post-bookings",
            "Create booking", "Creates a new **Booking**");
        
        swaggerDoc.ConfigureOperation(BookingIdEndpoint, HttpMethod.Get, "get-bookings-id",
            "List booking by id", "Return a **Booking** by the given id");
        
        swaggerDoc.ConfigureOperation(BookingIdEndpoint, HttpMethod.Patch, "patch-bookings-id",
            "Update booking", "Update **Booking** by the given id");
        
        swaggerDoc.ConfigureOperation(BookingIdStatusEndpoint, HttpMethod.Patch,  "patch-bookings-id-status",
            "Update booking status", "Update **Booking** status by the given id");
        
        swaggerDoc.ConfigureOperation(BookingCustomerEndpoint, HttpMethod.Get,"get-bookings-customer",
            "List bookings by customer",  "Return a list of **Bookings** for a specific customer, it can be filtered by the page number and page size");
    }
}