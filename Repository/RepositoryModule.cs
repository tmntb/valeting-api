using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Entities;
using Repository.Repositories;
using Service.Interfaces;

namespace Repository;

/// <summary>
/// Registers repository and database context dependencies.
/// </summary>
[ExcludeFromCodeCoverage]
public static class RepositoryModule
{
    /// <summary>
    /// Registers the Entity Framework context and repository implementations.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application settings containing the database connection string.</param>
    public static void AddRepository(this IServiceCollection services, IConfiguration configuration)
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

        services
            .AddScoped<IBookingRepository, BookingRepository>()
            .AddScoped<IFlexibilityRepository, FlexibilityRepository>()
            .AddScoped<IWebAuthnRepository, WebAuthnRepository>()
            .AddScoped<IRecoveryCodeRepository, RecoveryCodeRepository>()
            .AddScoped<IRoleRepository, RoleRepository>()
            .AddScoped<IStatusRepository, StatusRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IVehicleSizeRepository, VehicleSizeRepository>();
    }
}