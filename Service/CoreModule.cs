using Microsoft.Extensions.DependencyInjection;
using Common.Cache;
using Service.Services;
using Service.Interfaces;
using Common.Cache.Interfaces;
using Service.Validators.Utils;

namespace Service;

public static class CoreModule
{
    public static void AddValetingCore(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<ICacheHandler, MemoryCacheHandler>();

        services.AddScoped<ValidationHelpers>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IFlexibilityService, FlexibilityService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVehicleSizeService, VehicleSizeService>();
        services.AddScoped<IUrlService, UrlService>();
    }
}