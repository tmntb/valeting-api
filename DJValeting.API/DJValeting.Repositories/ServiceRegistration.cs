using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using DJValeting.Repositories.Entities;
using DJValeting.Repositories.Interfaces;

namespace DJValeting.Repositories
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DJValetingContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(DJValetingContext).Assembly.FullName)));

            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IFlexibilityRepository, FlexibilityRepository>();
            services.AddScoped<IVehicleSizeRepository, VehicleSizeRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
