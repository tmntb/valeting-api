using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;

namespace Api.RateLimiting;

public sealed class TestLimitRule(/*ValetingApiContext valetingApiContext,*/ ILogger<TestLimitRule> logger) : IRateLimitRule
{
    private readonly ConcurrentDictionary<string, TestThrottleState> _tracker = new();

    public string PolicyName => "Teste Rule";

    public void Configure(RateLimiterOptions options) => options.AddPolicy(PolicyName, CreateLimiter);

    /// <summary>
    /// Creates a rate limiter partition based on the incoming Http context, using a throttle key derived from the user's identity or IP address.
    /// The method builds a unique request hash based on the Http method, path and query parameters to track distinct requests and determine if a penalty should be applied.
    /// </summary>
    /// <param name="httpContext">The Http context of the incoming request.</param>
    /// <returns>A rate limiter partition for the given Http context.</returns>
    private RateLimitPartition<string> CreateLimiter(HttpContext httpContext)
    {
        var throttleKey = ResolveThrottleKey(httpContext);
        var requestHash = BuildRequestHash(httpContext.Request);
        var isPenalty = UpdateThrottleState(throttleKey, requestHash);

        logger.LogInformation("Rate limit evaluation for {PolicyName}: throttle key {ThrottleKey} resolved to {Tier} tier with request hash {RequestHash}.",
            PolicyName, throttleKey, isPenalty ? "penalty" : "base", requestHash);

        var partitionKey = isPenalty ? $"{PolicyName}:penalty:{throttleKey}" : $"{PolicyName}:base:{throttleKey}";

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = isPenalty ? 1 : 0, //penalty limit, base limit
            Window = TimeSpan.FromMinutes(2), //time window
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            AutoReplenishment = true
        });
    }

    /// <summary>
    /// Updates the throttle state for the given identity key, incrementing the request count and recording the request fingerprint.
    /// If the accumulated counts exceed the enumeration or rate thresholds, escalates the identity to penalty tier.
    /// </summary>
    /// <param name="throttleKey">The identity key for the client.</param>
    /// <param name="requestHash">The normalized fingerprint of the incoming request.</param>
    /// <returns><c>true</c> if the client is currently in the penalty tier, otherwise <c>false</c></returns>
    private bool UpdateThrottleState(string throttleKey, string requestHash)
    {
        var now = DateTimeOffset.UtcNow;
        var trackerWindow = TimeSpan.FromMinutes(3); //timie tracker window
        var throttleState = _tracker.GetOrAdd(throttleKey, _ => new TestThrottleState(now, now.Add(trackerWindow)));

        lock (throttleState.SyncRoot)
        {
            var windowWasExpired = now >= throttleState.WindowEndsAtUtc;
            ResetStateIfWindowExpired(throttleState, now, trackerWindow);

            throttleState.RequestCount++;
            throttleState.UniqueRequests.Add(requestHash);

            if (windowWasExpired)
            {
                logger.LogInformation("Throttle window reset for {ThrottleKey}. Tracking resumes until {WindowEndsAtUtc:o}.", throttleKey, throttleState.WindowEndsAtUtc);
            }

            if (ShouldEscalate(throttleState))
            {
                var penaltyWindow = TimeSpan.FromMinutes(2); //penalty window
                throttleState.PenaltyUntilUtc = now.Add(penaltyWindow);

                logger.LogWarning("Throttle key {ThrottleKey} escalated to penalty unitl {PenaltyUntilUtc:o}. Unique requests: {UniqueRequests}. Total Requests: {RequestCount}.",
                    throttleKey, throttleState.PenaltyUntilUtc, throttleState.UniqueRequests.Count, throttleState.RequestCount);
            }

            return throttleState.PenaltyUntilUtc > now;
        }
    }

    /// <summary>
    /// Determines whether the client should be escalated to the penalty tier on the current throttle state.
    /// Escalation is triggered when the number of unique requests fingerprints reaches 40 (enumeration pattern) or the total request count reaches 120 (burst pattern) within the tracking window.
    /// </summary>
    /// <param name="throttleState">The current throttle state for the client.</param>
    /// <returns><c>true</c> if either escalation threshold has been reached, otherwise <c>false</c></returns>
    private bool ShouldEscalate(TestThrottleState throttleState)
    {
        var reachedRouteCardinaltyThreshold = throttleState.UniqueRequests.Count >= 40;
        var reachedRequestRateThreshold = throttleState.RequestCount >= 120;
        return reachedRouteCardinaltyThreshold || reachedRequestRateThreshold;
    }

    /// <summary>
    /// Resets the tracking counters (request count and unique request set) if the current tracking window has expired and advances the window start time.
    /// </summary>
    /// <param name="throttleState">The throttle state to evaluate and potentially reset.</param>
    /// <param name="now">The current UTC time.</param>
    /// <param name="trackerWindow">The durantion of the tracking window.</param>
    private void ResetStateIfWindowExpired(TestThrottleState throttleState, DateTimeOffset now, TimeSpan trackerWindow)
    {
        if (now < throttleState.WindowEndsAtUtc)
            return;

        throttleState.RequestCount = 0;
        throttleState.UniqueRequests.Clear();
        throttleState.WindowEndsAtUtc = now.Add(trackerWindow);
    }

    /// <summary>
    /// Build a deterministic SHA-256 fingerprint from the Http method, path and sorted query parameters.
    /// Parameters and values are normalized so that equivalent requests always produce the same hash regardless of the ordering.
    /// </summary>
    /// <param name="request">The incoming Http request.</param>
    /// <returns>A hex-encoded SHA-256 hash representing the canonical form of the request.</returns>
    private static string BuildRequestHash(HttpRequest request)
    {
        var canonicaRequest = new StringBuilder()
            .Append(request.Method.ToUpperInvariant())
            .Append('|')
            .Append(request.Path.Value?.ToUpperInvariant() ?? string.Empty);

        foreach (var queryParameter in request.Query.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
        {
            canonicaRequest = new StringBuilder()
                .Append('|')
                .Append(queryParameter.Key.ToUpperInvariant())
                .Append('=')
                .Append(string.Join(',', queryParameter.Value.Select(value => value?.Trim() ?? string.Empty).OrderBy(value => value, StringComparer.OrdinalIgnoreCase)));
        }

        var requestBytes = Encoding.UTF8.GetBytes(canonicaRequest.ToString());
        var hashBytes = SHA256.HashData(requestBytes);
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// Resolves a stable identify key for the client. Uses the Jwt <c>NameIdentifier</c> claim when present, falls back to the remote IP address otherwise.
    /// </summary>
    /// <param name="httpContext">The current Http context.</param>
    /// <returns>A string key prefixed with <c>sub:</c> or <c>ip:</c> depending on which signal is available.</returns>
    private static string ResolveThrottleKey(HttpContext httpContext)
    {
        //substituir pelo valeting api context
        var subject = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(subject))
            return $"sub:{subject}";

        return $"ip:{httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";
    }

    private sealed class TestThrottleState(DateTimeOffset createdAtUtc, DateTimeOffset windowEndsAtUtc)
    {
        public object SyncRoot { get; } = new();
        public int RequestCount { get; set; }
        public HashSet<string> UniqueRequests { get; } = [];
        public DateTimeOffset PenaltyUntilUtc { get; set; } = createdAtUtc;
        public DateTimeOffset WindowEndsAtUtc { get; set; } = windowEndsAtUtc;
    }

}
