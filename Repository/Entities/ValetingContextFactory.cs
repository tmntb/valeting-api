using System.Diagnostics.CodeAnalysis;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Repository.Entities;

[ExcludeFromCodeCoverage]
public class ValetingContextFactory : IDesignTimeDbContextFactory<ValetingContext>
{
    public ValetingContext CreateDbContext(string[] args)
    {
        var envPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));
        Env.Load(envPath);
        Console.WriteLine("✅ .env file loaded successfully");

        // Load appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var saPassword = Environment.GetEnvironmentVariable("SA_PASSWORD") ?? string.Empty;

        // Re-write connection string with the environment password
        var connectionString = configuration.GetConnectionString("ValetingConnection");
        if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(saPassword))
        {
            // Replace placeholder with the real password
            connectionString = connectionString.Replace("{SA_PASSWORD}", saPassword);
            configuration["ConnectionStrings:ValetingConnection"] = connectionString;
        }

        var optionsBuilder = new DbContextOptionsBuilder<ValetingContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ValetingContext(optionsBuilder.Options);
    }
}
