using System.Diagnostics.CodeAnalysis;
using Common.Cache;
using Common.Cache.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resend;
using Service.Interfaces;
using Service.Services;
using Service.Validators.Utils;

namespace Service;

/// <summary>
/// Registers service-layer dependencies in the application's dependency injection container.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceModule
{
    /// <summary>
    /// Registers application services, validators, challenge storage, and the cache handler.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The configuration instance.</param>
    public static void AddService(this IServiceCollection services, IConfiguration configuration)
    {
        // Caching
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
            options.InstanceName = "Valeting_";
        });
        
        // ReSend
        services.AddHttpClient<ResendClient>();
        services.Configure<ResendClientOptions>( o =>
        {
            o.ApiToken = configuration["ReSendApiKey"]!;
        } );
        services.AddTransient<IResend, ResendClient>();
        
        services.AddSingleton<ICacheHandler, RedisCacheHandler>();
        
        services.AddScoped<IWebAuthnChallengeStore, WebAuthnChallengeStore>();

        services.AddScoped<ValidationHelpers>();

        // Services
        services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IEmailService, EmailService>()
            .AddScoped<IBookingService, BookingService>()
            .AddScoped<IFlexibilityService, FlexibilityService>()
            .AddScoped<ILinkService, LinkService>()
            .AddScoped<IMfaService, MfaService>()
            .AddScoped<IRecoveryCodeService, RecoveryCodeService>()
            .AddScoped<ITokenService, TokenService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IVehicleSizeService, VehicleSizeService>()
            .AddScoped<IWebAuthnService, WebAuthnService>();
    }
}