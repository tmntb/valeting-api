using Microsoft.Extensions.DependencyInjection;
using Valeting.Common.Cache;
using Valeting.Core.Services;
using Valeting.Core.Interfaces;
using Valeting.Common.Cache.Interfaces;
using Valeting.Core.Validators.Utils;

namespace Valeting.Core;

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