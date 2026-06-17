using Microsoft.Extensions.Caching.Distributed;

namespace AcaHelpAPI.Services
{
    public class RedisService : ICacheService
    {

        private readonly IDistributedCache _cache;
        public RedisService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetItemInCache<T>(string key)
        {
          var element = await _cache.GetStringAsync(key);
          if (element == null)
          {
              return default(T);
          }
          return System.Text.Json.JsonSerializer.Deserialize<T>(element);
        }

        public async Task SetItemInCache<T>(string key, T item, TimeSpan? expirationTime = null)
        {
            var options = new DistributedCacheEntryOptions();
            if (expirationTime.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expirationTime.Value;
            }
            var serializedItem = System.Text.Json.JsonSerializer.Serialize(item);
            await _cache.SetStringAsync(key, serializedItem, options);

        }

        public async Task RemoveItem(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
