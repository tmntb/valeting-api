namespace Common.Cache;

public class CacheOptions
{
    public Guid? Id { get; set; }
    public CacheListType? ListType { get; set; }
    public TimeSpan? AbsoluteExpireTime { get; set; }
    public TimeSpan? SlidingExpireTime { get; set; }
}
