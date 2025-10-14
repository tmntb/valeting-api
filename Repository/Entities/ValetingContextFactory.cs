using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Repository.Entities;

public class ValetingContextFactory : IDesignTimeDbContextFactory<ValetingContext>
{
    public ValetingContext CreateDbContext(string[] args)
    {
        // Carrega o appsettings.Development.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Certifica-te de que este caminho está correto
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("ValetingConnection");

        var optionsBuilder = new DbContextOptionsBuilder<ValetingContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ValetingContext(optionsBuilder.Options);
    }
}
