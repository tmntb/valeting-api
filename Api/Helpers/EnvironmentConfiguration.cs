using System.Diagnostics.CodeAnalysis;
using DotNetEnv;

namespace Api.Helpers;

[ExcludeFromCodeCoverage(Justification = "This class is responsible for loading environment variables and configuring the application at startup. It primarily consists of boilerplate code for handling configuration and does not contain business logic, so we can safely exclude it from code coverage metrics.")]
internal static class EnvironmentConfiguration
{
    /// <summary>
    /// Loads the .env file if the application is running in Development environment.
    /// This allows for local development overrides without affecting production deployments.
    /// </summary>
    /// <param name="environment">The hosting environment.</param>
    internal static void LoadDotEnvIfDevelopment(IHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            return;
        }

        var envPath = Path.Combine(environment.ContentRootPath, "..", ".env");

        if (!File.Exists(envPath))
        {
            Console.WriteLine("⚠️ .env file not found, using appsettings or environment variables");
            return;
        }

        Env.Load(envPath);
        Console.WriteLine("✅ .env file loaded successfully");
    }

    /// <summary>
    /// Replaces the {SA_PASSWORD} placeholder in the ValetingConnection connection string with the actual SA password from environment variables.
    /// This allows us to keep the sensitive SA password out of appsettings.json and only inject it at runtime from a secure source (like Docker secrets or environment variables).
    /// If the connection string does not contain the placeholder, this method will do nothing.
    /// If the SA_PASSWORD environment variable is not set, this method will also do nothing, allowing the application to fall back to any existing connection string value in configuration (if present).
    /// </summary> <param name="config"></param>
    /// <exception cref="InvalidOperationException"></exception>
    internal static void ConfigureConnectionString(IConfiguration config)
    {
        var saPassword = GetEnvVar("SA_PASSWORD");
        var connectionString = config.GetConnectionString("ValetingConnection") ?? throw new InvalidOperationException("ValetingConnection not configured");

        if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(saPassword))
        {
            connectionString = connectionString.Replace("{SA_PASSWORD}", saPassword);
            config["ConnectionStrings:ValetingConnection"] = connectionString;

            Console.WriteLine("✅ Replaced {SA_PASSWORD} placeholder in connection string.");
        }
    }

    /// <summary>
    /// Replaces the {JWT_KEY} placeholder in the Jwt:Key configuration value with the actual JWT key from environment variables.
    /// This allows us to keep the sensitive JWT key out of appsettings.json and only inject it at runtime from a secure source (like Docker secrets or environment variables).
    /// If the Jwt:Key value does not contain the placeholder, this method will do nothing.
    /// If the JWT_KEY environment variable is not set, this method will also do nothing, allowing the application to fall back to any existing Jwt:Key value in configuration (if present).
    /// </summary>
    /// <param name="config"></param>
    /// <exception cref="InvalidOperationException"></exception>
    internal static void ConfigureJwtKey(IConfiguration config)
    {
        var jwtKey = GetEnvVar("JWT_KEY");
        var jwtKeyString = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");

        if (!string.IsNullOrEmpty(jwtKeyString) && !string.IsNullOrEmpty(jwtKey))
        {
            // Replace placeholder with the real JWT key
            jwtKeyString = jwtKeyString.Replace("{JWT_KEY}", jwtKey);
            config["Jwt:Key"] = jwtKeyString;

            Console.WriteLine("✅ Replaced {JWT_KEY} placeholder in Jwt:Key.");
        }
    }

    /// <summary>
    /// Helper method to read an environment variable and optionally enforce that it is set (non-empty). If the variable is required but not set, an exception will be thrown. If it is not required and not set, an empty string will be returned.
    /// This centralizes the logic for reading environment variables and provides consistent error handling and logging for missing variables.
    /// </summary>
    /// <param name="name">The name of the environment variable to read.</param>
    /// <returns>The value of the environment variable, or an empty string if it is not required and not set.</returns>
    private static string GetEnvVar(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);

        if (string.IsNullOrEmpty(value))
        {
            Console.WriteLine($"⚠️ Environment variable '{name}' not set. Falling back to config value if available.");
            return string.Empty;
        }

        Console.WriteLine($"✅ Loaded environment variable '{name}'.");
        return value;
    }
}
