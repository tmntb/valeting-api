using System.Diagnostics.CodeAnalysis;
using Common.Cache;
using Common.Cache.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;
using Service.Services;
using Service.Validators.Utils;

namespace Service;

[ExcludeFromCodeCoverage]
public static class ServiceModule
{
    public static void AddService(this IServiceCollection services)
    {
        // Caching
        services.AddMemoryCache();
        services.AddScoped<ICacheHandler, MemoryCacheHandler>();
        services.AddScoped<IPasskeyChallengeStore, PasskeyChallengeStore>();

        services.AddScoped<ValidationHelpers>();

        // Services
        services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IBookingService, BookingService>()
            .AddScoped<IFlexibilityService, FlexibilityService>()
            .AddScoped<ILinkService, LinkService>()
            .AddScoped<IMfaService, MfaService>()
            .AddScoped<IPasskeyService, PasskeyService>()
            .AddScoped<IRecoveryCodeService, RecoveryCodeService>()
            .AddScoped<ITokenService, TokenService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IVehicleSizeService, VehicleSizeService>();
    }
}