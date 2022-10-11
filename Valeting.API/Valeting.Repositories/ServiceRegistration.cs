using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Valeting.Repositories.Entities;
using Valeting.Repositories.Interfaces;

namespace Valeting.Repositories
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ValetingContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ValetingContext).Assembly.FullName)
                )
            );

            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IFlexibilityRepository, FlexibilityRepository>();
            services.AddScoped<IVehicleSizeRepository, VehicleSizeRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
