using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Api.Helpers;
using Api.Middleware;
using Api.SwaggerDocumentation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Service;

var builder = WebApplication.CreateBuilder(args);

// Load .env ONLY in local development (outside of Docker)
EnvironmentConfiguration.LoadDotEnvIfDevelopment(builder.Environment);

var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
var appSettingsPath = Path.Combine(assemblyLocation, "appsettings.json");
var appSettingsDevPath = Path.Combine(assemblyLocation, $"appsettings.{builder.Environment.EnvironmentName}.json");
builder.Configuration
    .AddJsonFile(appSettingsPath, optional: true, reloadOnChange: true)
    .AddJsonFile(appSettingsDevPath, optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Apply overrides from environment variables (if present)
EnvironmentConfiguration.ConfigureConnectionString(builder.Configuration);
EnvironmentConfiguration.ConfigureJwtKey(builder.Configuration);

// Add services to the container.
builder.Services.AddService();
builder.Services.AddRepository(builder.Configuration);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services
    .AddAuthorizationBuilder()
    .AddPolicy("RequireJwt", policy =>
    {
        policy.AuthenticationSchemes.Add("JwtBearer");
        policy.RequireAuthenticatedUser();
    });

builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddScoped<ExceptionHandlingMiddleware>();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UsePathBase("/valeting");

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseDefaultFiles();

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
    c.SwaggerEndpoint("v1/swagger.json", "Valeting v1");
});

app.Run();

[ExcludeFromCodeCoverage]
public partial class Program { }
