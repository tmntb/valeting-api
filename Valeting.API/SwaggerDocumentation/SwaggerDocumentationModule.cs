using Microsoft.OpenApi.Models;
using Valeting.API.SwaggerDocumentation.Document;
using Valeting.API.SwaggerDocumentation.Parameter;

namespace Valeting.API.SwaggerDocumentation;

public static class SwaggerDocumentationExtensions
{
    public static void AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Valeting",
                Version = "v1",
                Description = "This is an API to and manage your reserves for cars.",
                Contact = new OpenApiContact
                {
                    Name = "Tiago Baeta",
                    Email = "tmntb.work@gmail.com"
                }
            });

            c.AddServer(new OpenApiServer { Description = "Local", Url = "https://localhost:44376/Valeting" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            { 
                Name = "Authorization", 
                Type = SecuritySchemeType.ApiKey, 
                Scheme = "Bearer", 
                BearerFormat = "JWT", 
                In = ParameterLocation.Header, 
                Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"", 
            }); 
            c.AddSecurityRequirement(new OpenApiSecurityRequirement 
            { 
                { 
                    new OpenApiSecurityScheme 
                    { 
                        Reference = new OpenApiReference 
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