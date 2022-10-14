using Microsoft.Extensions.DependencyInjection;

using Valeting.Service;
using Valeting.Services.Interfaces;

namespace Valeting.Services
{
    public static class ServiceRegistration
    {
        public static void AddMiddleware(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IFlexibilityService, FlexibilityService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVehicleSizeService, VehicleSizeService>();
        }
    }
}

