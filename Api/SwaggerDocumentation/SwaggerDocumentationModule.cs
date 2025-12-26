using System.Diagnostics.CodeAnalysis;
using Api.SwaggerDocumentation.Document;
using Api.SwaggerDocumentation.Parameter;
using Microsoft.OpenApi.Models;

namespace Api.SwaggerDocumentation;

/// <summary>
/// Provides extension methods to configure Swagger/OpenAPI documentation for the Valeting API.
/// </summary>
[ExcludeFromCodeCoverage]
public static class SwaggerDocumentationExtensions
{
    /// <summary>
    /// Adds and configures Swagger documentation, including API info, servers, security, and custom filters.
    /// </summary>
    /// <param name="services">The service collection to which Swagger will be added.</param>
    public static void AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SupportNonNullableReferenceTypes();

            c.SwaggerDoc("v1", new()
            {
                Title = "Valeting",
                Version = "v1",
                Description = "This is an API to manage car bookings.",
                Contact = new()
                {
                    Name = "Tiago Baeta",
                    Email = "tmntb.work@gmail.com"
                }
            });

            c.AddServer(new() { Description = "Local", Url = "https://localhost:44376/valeting" });
            c.AddServer(new() { Description = "Docker", Url = "https://localhost:8080/valeting" });

            c.AddSecurityDefinition("Bearer", new()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme.\r\n\r\nEnter 'Bearer' [space] and then your token.\r\nExample: \"Bearer 12345abcdef\""
            });
            c.AddSecurityRequirement(new()
            {
                {
                    new()
                    {
                        Reference = new()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            c.DocumentFilter<BookingDocumentFilter>();
            c.DocumentFilter<FlexibilityDocumentFilter>();
            c.DocumentFilter<VehicleSizeDocumentFilter>();
            c.DocumentFilter<UserDocumentFilter>();

            c.ParameterFilter<ParameterFilter>();
        });
    }
}
