namespace Common.Cache;

/// <summary>
/// Represents configuration options for caching data in the system.
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// Optional identifier for a specific cache entry.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Optional type of entity list for which the cache is being used.
    /// </summary>
    public CacheListType? ListType { get; set; }

    /// <summary>
    /// Optional absolute expiration time for the cache entry.
    /// </summary>
    public TimeSpan? AbsoluteExpireTime { get; set; }

    /// <summary>
    /// Optional sliding expiration time for the cache entry.
    /// </summary>
    public TimeSpan? SlidingExpireTime { get; set; }
}
