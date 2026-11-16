using System.Diagnostics.CodeAnalysis;
using Common.Cache;
using Common.Cache.Interfaces;
using Microsoft.Extensions.DependencyInjection;
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
    public static void AddService(this IServiceCollection services)
    {
        // Caching
        services.AddScoped<ICacheHandler, RedisCacheHandler>();
        services.AddScoped<IWebAuthnChallengeStore, WebAuthnChallengeStore>();

        services.AddScoped<ValidationHelpers>();

        // Services
        services
            .AddScoped<IAuthService, AuthService>()
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