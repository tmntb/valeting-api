using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Valeting.Repository.Entities;
using Valeting.Repository.Interfaces;
using Valeting.Repository.Repositories;

namespace Valeting.Repository;

public static class RepositoryModule
{
    public static void AddValetingRepository(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ValetingContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("ValetingConnection"),
                b =>
                {
                    b.MigrationsAssembly(typeof(ValetingContext).Assembly.FullName);
                    b.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
                }
            )
        );

        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IFlexibilityRepository, FlexibilityRepository>();
        services.AddScoped<IVehicleSizeRepository, VehicleSizeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}