using System;
using Microsoft.Extensions.Caching.Memory;

namespace Equinor.Procosys.Preservation.Infrastructure.Caching
{
    public class CacheManager : ICacheManager
    {
        private readonly IMemoryCache _cache;

        public CacheManager() => _cache = new MemoryCache(new MemoryCacheOptions());

        public T Get<T>(string key) where T : class => _cache.Get(key) as T;

        public T GetOrCreate<T>(string key, Func<T> fetch, CacheDuration duration, long expiration) where T : class
        {
            var instance = Get<T>(key);
            if (instance != null)
            {
                return instance;
            }

            instance = fetch.Invoke();
            Add(key, instance, duration, expiration);
            return instance;
        }

        private void Add<T>(string key, T instance, CacheDuration duration, long expiration) where T : class
        {
            if (instance == null)
            {
                return;
            }

            _cache.Set(key, instance, DateTime.UtcNow.Add(GetExpirationTime(duration, expiration)));
        }

        private static TimeSpan GetExpirationTime(CacheDuration duration, long expiration)
        {
            switch (duration)
            {
                case CacheDuration.Hours:
                    return TimeSpan.FromHours(expiration);
                case CacheDuration.Minutes:
                    return TimeSpan.FromMinutes(expiration);
                case CacheDuration.Seconds:
                    return TimeSpan.FromSeconds(expiration);
                default:
                    throw new NotImplementedException($"Unknown {nameof(CacheDuration)}: {duration}");
            }
        }
    }
}
