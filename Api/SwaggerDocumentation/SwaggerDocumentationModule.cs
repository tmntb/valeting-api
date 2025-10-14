using Microsoft.OpenApi.Models;
using Api.SwaggerDocumentation.Document;
using Api.SwaggerDocumentation.Parameter;

namespace Api.SwaggerDocumentation;

public static class SwaggerDocumentationExtensions
{
    public static void AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new()
            {
                Title = "Valeting",
                Version = "v1",
                Description = "This is an API to and manage your reserves for cars.",
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
                Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            });
            c.AddSecurityRequirement(new()
            {
                {
                    new ()
                    {
                        Reference = new()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
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