using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Concurrent;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;

namespace Api.RateLimiting;

public sealed class LoginLimitRule(IOptions<RateLimitConfig> rateLimitConfig, ILogger<LoginLimitRule> logger) : IRateLimitRule
{
    private readonly LoginRateLimitConfig _loginRateLimitConfig = rateLimitConfig.Value.Login;
    private readonly ConcurrentDictionary<string, ThrottleState> _tracker = new();

    public string PolicyName => "Login";

    public void Configure(RateLimiterOptions options) => options.AddPolicy(PolicyName, CreateLimiter);

    /// <summary>
    /// Creates a rate limiter partition based on the incoming Http context, using a throttle key derived from the user's identity or IP address.
    /// The method builds a unique request hash based on the Http method, path and query parameters to track distinct requests and determine if a penalty should be applied.
    /// </summary>
    /// <param name="httpContext">The Http context of the incoming request.</param>
    /// <returns>A rate limiter partition for the given Http context.</returns>
    private RateLimitPartition<string> CreateLimiter(HttpContext httpContext)
    {
        var throttleKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var isPenalty = UpdateThrottleState(throttleKey);

        logger.LogInformation("Rate limit evaluation for {PolicyName}: throttle key {ThrottleKey} resolved to {Tier} tier.",
            PolicyName, throttleKey, isPenalty ? "penalty" : "base");

        var partitionKey = isPenalty ? $"{PolicyName}:penalty:{throttleKey}" : $"{PolicyName}:base:{throttleKey}";

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = isPenalty ? _loginRateLimitConfig.Penalty.PermitLimit : _loginRateLimitConfig.Base.PermitLimit,
            Window = isPenalty ? _loginRateLimitConfig.Penalty.Window : _loginRateLimitConfig.Base.Window,
            QueueLimit = _loginRateLimitConfig.QueueLimit,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            AutoReplenishment = true
        });
    }

    /// <summary>
    /// Updates the throttle state for the given identity key, incrementing the request count and recording the request fingerprint.
    /// If the accumulated counts exceed the enumeration or rate thresholds, escalates the identity to penalty tier.
    /// </summary>
    /// <param name="throttleKey">The identity key for the client.</param>
    /// <returns><c>true</c> if the client is currently in the penalty tier, otherwise <c>false</c></returns>
    private bool UpdateThrottleState(string throttleKey)
    {
        var now = DateTimeOffset.UtcNow;
        var trackerWindow = _loginRateLimitConfig.TrackerWindow;
        var throttleState = _tracker.GetOrAdd(throttleKey, _ => new ThrottleState(now.Add(trackerWindow)));

        lock (throttleState.SyncRoot)
        {
            if (throttleState.PenaltyUntilUtc > now)
                return true;
            
            if (now >= throttleState.TrackingWindowEndsAtUtc)
            {
                logger.LogInformation("Throttle window reset for {ThrottleKey}. Tracking resumes until {WindowEndsAtUtc:o}.", throttleKey, throttleState.TrackingWindowEndsAtUtc);
                
                throttleState.RequestCount = 0;
                throttleState.TrackingWindowEndsAtUtc = now.Add(trackerWindow);
            }

            throttleState.RequestCount++;

            if (throttleState.RequestCount < 6)
                return false;
            
            throttleState.PenaltyUntilUtc = now.Add(_loginRateLimitConfig.PenaltyWindow);
            
            logger.LogWarning("Throttle key {ThrottleKey} escalated to penalty until {PenaltyUntilUtc:o}. Total Requests: {RequestCount}.",
                throttleKey, throttleState.PenaltyUntilUtc, throttleState.RequestCount);

            return true;
        }
    }

    private sealed class ThrottleState(DateTimeOffset trackingWindowEndsAtUtc)
    {
        public object SyncRoot { get; } = new();
        public int RequestCount { get; set; }
        public DateTimeOffset TrackingWindowEndsAtUtc { get; set; } = trackingWindowEndsAtUtc;
        public DateTimeOffset PenaltyUntilUtc { get; set; }
    }
}
