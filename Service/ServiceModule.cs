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
        services.AddMemoryCache();
        services.AddScoped<ICacheHandler, MemoryCacheHandler>();

        services.AddScoped<ValidationHelpers>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IFlexibilityService, FlexibilityService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVehicleSizeService, VehicleSizeService>();
        services.AddScoped<ILinkService, LinkService>();
    }
}