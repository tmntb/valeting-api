using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.RateLimiting;

[ExcludeFromCodeCoverage]
public static class RateLimitingModule
{
    /// <summary>
    /// Extension method to register a rate limit rule implementation in the service collection.
    /// This allows for a modular addition of rate limiting rules by simply implementing the IRateLimitRule interface.
    /// </summary>
    /// <typeparam name="TRule">The type of the rate limit rule to register, which must implement IRateLimitRule.</typeparam>
    /// <param name="services">The service collection to which the rate limit rule will be added.</param>
    /// <returns>The updated service collection with the rate limit rule registered.</returns>
    public static IServiceCollection AddRateLimitingRule<TRule>(this IServiceCollection services) where TRule : class, IRateLimitRule
    {
        services.AddSingleton<IRateLimitRule, TRule>();
        return services;
    }

    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = (context, cancellationToken) => HandleRejected(context.HttpContext, cancellationToken);
        });

        services
            .AddOptions<RateLimiterOptions>()
            .Configure<IEnumerable<IRateLimitRule>>((options, rules) =>
            {
                foreach (var rule in rules)
                {
                    rule.Configure(options);
                }
            });

        return services;
    }

    private static ValueTask HandleRejected(HttpContext httpContext, CancellationToken cancellationToken)
    {
        var response = httpContext.Response;
        response.StatusCode = StatusCodes.Status429TooManyRequests;

        return new ValueTask(response.WriteAsJsonAsync(new
        {
            message = "Too many requests. Please retry later."
        }, cancellationToken));
    }
}
