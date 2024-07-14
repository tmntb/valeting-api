using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Valeting.Repository.Entities;
using Valeting.Repository.Repositories;
using Valeting.Repository.Repositories.Interfaces;

namespace Valeting.Repository;

public static class RepositoryRegistration
{
    public static void AddInfrastructureDataLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ValetingContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("ValetingConnection"),
                b => b.MigrationsAssembly(typeof(ValetingContext).Assembly.FullName)
            )
        );

        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IFlexibilityRepository, FlexibilityRepository>();
        services.AddScoped<IVehicleSizeRepository, VehicleSizeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}