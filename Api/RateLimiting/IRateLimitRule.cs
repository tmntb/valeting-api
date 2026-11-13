using Microsoft.AspNetCore.RateLimiting;

namespace Api.RateLimiting;

public interface IRateLimitRule
{
    string PolicyName {  get; }

    /// <summary>
    /// Registers the rate limiting policy into the provided <see cref="RateLimiterOptions"/>.
    /// </summary>
    /// <param name="options">The rate limiter options to configure.</param>
    void Configure(RateLimiterOptions options);
}
