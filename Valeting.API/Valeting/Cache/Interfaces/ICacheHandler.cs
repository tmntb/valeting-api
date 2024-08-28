namespace Valeting.Cache.Interfaces;

public interface ICacheHandler
{
    void SetRecord<T>(string recordKey, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null);
    T? GetRecord<T>(string recordKey);
    void RemoveRecord(string recordKey);
    void RemoveRecordsWithPrefix(string prefix);
}