using Microsoft.Extensions.DependencyInjection;

using Valeting.Core.Services;
using Valeting.Core.Services.Interfaces;

namespace Valeting.Core;

public static class ServiceRegistration
{
    public static void AddServicesLayer(this IServiceCollection services)
    {
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IFlexibilityService, FlexibilityService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVehicleSizeService, VehicleSizeService>();
        services.AddScoped<IUrlService, UrlService>();
    }
}