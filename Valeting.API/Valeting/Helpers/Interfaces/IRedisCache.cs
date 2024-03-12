namespace Valeting.Helpers.Interfaces;

public interface IRedisCache
{
    Task SetRecordAsync<T>(string recordId, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null);
    Task<T?> GetRecordAsync<T>(string recordId);
    Task RemoveRecord(string recordId);
}