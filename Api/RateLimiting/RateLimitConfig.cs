namespace Api.RateLimiting;

public class RateLimitConfig
{
    public const string SectionName = "RateLimiting";

    public LoginRateLimitConfig Login { get; set; } = new();
}

public class LoginRateLimitConfig
{
    public RateLimitRuleConfig Base { get; set; } = new();
    public RateLimitRuleConfig Penalty { get; set; } = new();
    public int QueueLimit { get; set; }
    public TimeSpan TrackerWindow { get; set; }
    public TimeSpan PenaltyWindow { get; set; }
}

public class RateLimitRuleConfig
{
    public int PermitLimit { get; set; }
    public TimeSpan Window { get; set; }
}
