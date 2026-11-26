using System.Diagnostics.CodeAnalysis;
using DotNetEnv;

namespace Api.Helpers;

[ExcludeFromCodeCoverage(Justification =
    "This class is responsible for loading environment variables and configuring the application at startup. It primarily consists of boilerplate code for handling configuration and does not contain business logic, so we can safely exclude it from code coverage metrics.")]
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
    /// </summary>
    /// <param name="config">The application configuration containing the database settings.</param>
    /// <exception cref="InvalidOperationException">Thrown when <c>ConnectionStrings:ValetingConnection</c> not configured.</exception>
    internal static void ConfigureDatabase(IConfiguration config)
    {
        var saPassword = GetEnvVar("SA_PASSWORD");
        var connectionString = config.GetConnectionString("ValetingConnection") ?? throw new InvalidOperationException("ValetingConnection not configured");

        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(saPassword)) 
            return;
        
        connectionString = connectionString.Replace("{SA_PASSWORD}", saPassword);
        config["ConnectionStrings:ValetingConnection"] = connectionString;

        Console.WriteLine("✅ Replaced {SA_PASSWORD} placeholder in connection string.");
    }

    /// <summary>
    /// Replaces the {JWT_KEY} placeholder in the Jwt:Key configuration value with the actual JWT key from environment variables.
    /// This allows us to keep the sensitive JWT key out of appsettings.json and only inject it at runtime from a secure source (like Docker secrets or environment variables).
    /// If the Jwt:Key value does not contain the placeholder, this method will do nothing.
    /// If the JWT_KEY environment variable is not set, this method will also do nothing, allowing the application to fall back to any existing Jwt:Key value in configuration (if present).
    /// </summary>
    /// <param name="config">The application configuration containing the jwt settings.</param>
    /// <exception cref="InvalidOperationException">Thrown when <c>Jwt:Key</c> not configured.</exception>
    internal static void ConfigureJwt(IConfiguration config)
    {
        var jwtKey = GetEnvVar("JWT_KEY");
        var jwtKeyString = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");

        if (string.IsNullOrEmpty(jwtKeyString) || string.IsNullOrEmpty(jwtKey)) 
            return;
        
        jwtKeyString = jwtKeyString.Replace("{JWT_KEY}", jwtKey);
        config["Jwt:Key"] = jwtKeyString;

        Console.WriteLine("✅ Replaced {JWT_KEY} placeholder in Jwt:Key.");
    }

    /// <summary>
    /// Replaces the WebAuthn configuration placeholders with values from the
    /// <c>PK_RPID</c>, <c>PK_RPNAME</c>, and <c>PK_ORIGINS</c> environment variables.
    /// </summary>
    /// <param name="config">The application configuration containing the WebAuthn settings.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <c>WebAuthn:RpId</c>, <c>WebAuthn:RpName</c>, or <c>WebAuthn:Origins</c> is not configured.
    /// </exception>
    internal static void ConfigureWebAuthn(IConfiguration config)
    {
        var rpId = GetEnvVar("PK_RPID");
        var rpName = GetEnvVar("PK_RPNAME");
        var allowedOrigins = GetEnvVar("PK_ORIGINS");

        var rpIdString = config["WebAuthn:RpId"] ?? throw new InvalidOperationException("WebAuthn:RpId not configured");
        var rpNameString = config["WebAuthn:RpName"] ?? throw new InvalidOperationException("WebAuthn:RpName not configured");
        var allowedOriginsString = config["WebAuthn:Origins"] ?? throw new InvalidOperationException("WebAuthn:Origins not configured");

        if ((string.IsNullOrEmpty(rpId) || string.IsNullOrEmpty(rpIdString)) &&
            (string.IsNullOrEmpty(rpName) || string.IsNullOrEmpty(rpNameString)) &&
            (string.IsNullOrEmpty(allowedOrigins) || string.IsNullOrEmpty(allowedOriginsString)))
            return;

        rpIdString = rpIdString.Replace("{PK_RPID}", rpId);
        config["WebAuthn:RpId"] = rpIdString;

        Console.WriteLine("✅ Replaced {PK_RPID} placeholder in WebAuthn:RpId.");

        rpNameString = rpNameString.Replace("{PK_RPNAME}", rpName);
        config["WebAuthn:RpName"] = rpNameString;

        Console.WriteLine("✅ Replaced {PK_RPNAME} placeholder in WebAuthn:RpName.");

        allowedOriginsString = allowedOriginsString.Replace("{PK_ORIGINS}", allowedOrigins);
        config["WebAuthn:RpOrigins"] = allowedOriginsString;

        Console.WriteLine("✅ Replaced {PK_ORIGINS} placeholder in WebAuthn:RpOrigins.");
    }

    /// <summary>
    /// Replaces the {REDIS_CONNECTION} placeholder in the RedisConnection connection string with the actual Redis connection string from environment variables.
    /// This allows us to keep the sensitive Redis connection string out of appsettings.json and only inject it at runtime from a secure source (like Docker secrets or environment variables).
    /// If the connection string does not contain the placeholder, this method will do nothing.
    /// If the REDIS_CONNECTION environment variable is not set, this method will also do nothing, allowing the application to fall back to any existing connection string value in configuration (if present).
    /// </summary>
    /// <param name="config">The application configuration containing the Redis connection string.</param>
    /// <exception cref="InvalidOperationException">Throw when <c>RedisConnection</c> is not configured.</exception>
    internal static void ConfigureRedis(IConfiguration config)
    {
        var redisConnection = GetEnvVar("REDIS_CONNECTION");
        var redisConnectionStr = config.GetConnectionString("RedisConnection") ?? throw new InvalidOperationException("RedisConnection not configured");

        if (string.IsNullOrEmpty(redisConnectionStr) || string.IsNullOrEmpty(redisConnection))
            return;

        redisConnectionStr = redisConnectionStr.Replace("{REDIS_CONNECTION}", redisConnection);
        config["ConnectionStrings:RedisConnection"] = redisConnectionStr;
        
        Console.WriteLine("✅ Replaced {REDIS_CONNECTION} placeholder in connection string.");
    }

    /// <summary>
    /// Replaces the {PASSWORD_RESET_BASE_URL} placeholder in the PasswordReset config the actual base url string from environment variables.
    /// Replaces the {PASSWORD_RESET_LIFETIME_MINUTES} placeholder in the PasswordReset config the actual value string from environment variables.
    /// If the connection string does not contain the placeholder, this method will do nothing.
    /// </summary>
    /// <param name="config">The application configuration containing the PasswordReset settings.</param>
    /// <exception cref="InvalidOperationException">
    /// Throw when <c>PasswordReset:BaseUrl</c> or <c>PasswordReset:LifetimeMinutes</c> is not configured.
    /// </exception>
    internal static void ConfigurePasswordReset(IConfiguration config)
    {
        var baseUrl = GetEnvVar("PASSWORD_RESET_BASE_URL");
        var lifetime = GetEnvVar("PASSWORD_RESET_LIFETIME_MINUTES");

        var baseUrlStr = config["PasswordReset:BaseUrl"] ?? throw new InvalidOperationException("Password reset base url not configured");
        var lifetimeStr = config["PasswordReset:LifetimeMinutes"] ?? throw new InvalidOperationException("Password reset life time not configured");

        if ((string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(baseUrlStr)) &&
            (string.IsNullOrEmpty(lifetime) || string.IsNullOrEmpty(lifetimeStr)))
            return;

        baseUrlStr = baseUrlStr.Replace("{PASSWORD_RESET_BASE_URL}", baseUrl);
        config["PasswordReset:BaseUrl"] = baseUrlStr;

        Console.WriteLine("✅ Replaced {PASSWORD_RESET_BASE_URL} placeholder in PasswordReset:BaseUrl.");

        lifetimeStr = lifetimeStr.Replace("{PASSWORD_RESET_LIFETIME_MINUTES}", lifetime);
        config["PasswordReset:LifetimeMinutes"] = lifetimeStr;

        Console.WriteLine("✅ Replaced {PASSWORD_RESET_LIFETIME_MINUTES} placeholder in PasswordReset:LifetimeMinutes.");
    }
    
    /// <summary>
    /// Replaces the {RESEND_API_KEY} placeholder in the ReSend config the actual key string from environment variables.
    /// If the connection string does not contain the placeholder, this method will do nothing.
    /// </summary>
    /// <param name="config">The application configuration containing the ReSend settings.</param>
    /// <exception cref="InvalidOperationException">Thrown when <c>ConnectionStrings:ReSendApiKey</c> not configured.</exception>
    internal static void ConfigureReSend(IConfiguration config)
    {
        var reSendApiKey = GetEnvVar("RESEND_API_KEY");
        var reSendApiKeyStr = config["ReSendApiKey"] ?? throw new InvalidOperationException("ReSendApiKey not configured");

        if (string.IsNullOrEmpty(reSendApiKeyStr) || string.IsNullOrEmpty(reSendApiKey))
            return;
        
        reSendApiKeyStr = reSendApiKeyStr.Replace("{RESEND_API_KEY}", reSendApiKey);
        config["ReSendApiKey"] = reSendApiKeyStr;

        Console.WriteLine("✅ Replaced {RESEND_API_KEY} placeholder in ReSendApiKey.");
    }

    /// <summary>
    /// Replaces the RateLimiting configuration placeholders with values from the environment variables.
    /// </summary>
    /// <param name="config">The application configuration containing the rate limiting settings.</param>
    /// <exception cref="InvalidOperationException">Throw when <c>RateLimiting</c> is not configured.</exception>
    internal static void ConfigureRateLimiting(IConfiguration config)
    {
        var rateLimiting = GetEnvVar("RATE_LIMITING");
        var rateLimitingStr = config["RateLimiting"] ?? throw new InvalidOperationException("Rate Limiting not configured");

        if (string.IsNullOrEmpty(rateLimiting) || string.IsNullOrEmpty(rateLimitingStr)) 
            return;
        
        rateLimitingStr = rateLimitingStr.Replace("{RATE_LIMITING}", rateLimiting);
        config["RateLimiting"] = rateLimitingStr;
        Console.WriteLine("✅ Replaced {RATE_LIMITING} placeholder in RateLimiting.");
    }

    /// <summary>
    /// Helper method to read an environment variable and optionally enforce that it is set (non-empty). 
    /// This centralizes the logic for reading environment variables and provides consistent error handling and logging for missing variables.
    /// </summary>
    /// <param name="name">The name of the environment variable to read.</param>
    /// <returns>The value of the environment variable, or an empty string if it is not set.</returns>
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
