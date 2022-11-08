using System.Net;
using System.Text.Json;

using Microsoft.Extensions.Caching.Distributed;

using StackExchange.Redis;

using Valeting.Helpers.Interfaces;

namespace Valeting.Helpers
{
    public class RedisCache : IRedisCache
    {
        private IDistributedCache _cache;
        private IConfiguration _configuration;

        public RedisCache(IDistributedCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        public async Task SetRecordAsync<T>(string recordId, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions();

                options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);
                options.SlidingExpiration = slidingExpireTime;

                var jsonData = JsonSerializer.Serialize(data);
                await _cache.SetStringAsync(recordId, jsonData, options);
            }
            catch (Exception) { }
        }

        public async Task<T?> GetRecordAsync<T>(string recordId)
        {
            var jsonData = string.Empty;
            try
            {
                jsonData = await _cache.GetStringAsync(recordId);

                if (jsonData is null)
                {
                    return default(T);
                }
                return JsonSerializer.Deserialize<T>(jsonData);
            }
            catch (Exception) { }

            return default(T);
        }

        public async Task RemoveRecordAsync(string recordId)
        {
            try
            {
                var options = ConfigurationOptions.Parse(_configuration["ConnectionStrings:Redis"]);
                var connection = ConnectionMultiplexer.Connect(options);
                var db = connection.GetDatabase();
                var endPoint = connection.GetEndPoints().First();
                var keys = connection.GetServer(endPoint).Keys(pattern: recordId).ToList();
                if (keys.Any())
                {
                    foreach (var key in keys)
                    {
                        await db.KeyDeleteAsync(key);
                    }
                }
            }
            catch (Exception) { }
        }
    }
}

