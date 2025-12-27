using System.Reflection;
using System.Text;
using Api.Middleware;
using Api.SwaggerDocumentation;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Service;

var builder = WebApplication.CreateBuilder(args);

// Load .env ONLY in local development (outside of Docker)
if (builder.Environment.IsDevelopment())
{
    var rootPath = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName
                   ?? Directory.GetCurrentDirectory();
    var envPath = Path.Combine(rootPath, ".env");

    if (File.Exists(envPath))
    {
        Env.Load(envPath);
        Console.WriteLine("✅ .env file loaded successfully");
    }
    else
    {
        Console.WriteLine("⚠️ .env file not found, using appsettings or environment variables");
    }
}

var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
var configBasePath = Path.GetFullPath(Path.Combine(assemblyLocation, "..", "..", ".."));

var appSettingsPath = Path.Combine(configBasePath, "appsettings.json");

builder.Configuration
    .AddJsonFile(appSettingsPath, optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Read environment variables
var saPassword = Environment.GetEnvironmentVariable("SA_PASSWORD") ?? string.Empty;
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? string.Empty;

// Re-write connection string with the environment password
var connectionString = builder.Configuration.GetConnectionString("ValetingConnection") ?? throw new InvalidOperationException("ValetingConnection not configured");
if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(saPassword))
{
    // Replace placeholder with the real password
    connectionString = connectionString.Replace("{SA_PASSWORD}", saPassword);
    builder.Configuration["ConnectionStrings:ValetingConnection"] = connectionString;
}

var jwtKeyString = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
if (!string.IsNullOrEmpty(jwtKeyString) && !string.IsNullOrEmpty(jwtKey))
{
    // Replace placeholder with the real JWT key
    jwtKeyString = jwtKeyString.Replace("{JWT_KEY}", jwtKey);
    builder.Configuration["Jwt:Key"] = jwtKeyString;
}

// Add services to the container.
builder.Services.AddService();
builder.Services.AddRepository(builder.Configuration);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireJwt", policy =>
    {
        policy.AuthenticationSchemes.Add("JwtBearer");
        policy.RequireAuthenticatedUser();
    });

builder.Services.AddControllers();

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
