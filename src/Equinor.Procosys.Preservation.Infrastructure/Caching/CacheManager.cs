using System;
using Equinor.Procosys.Preservation.Domain.Time;
using Microsoft.Extensions.Caching.Memory;

namespace Equinor.Procosys.Preservation.Infrastructure.Caching
{
    public class CacheManager : ICacheManager
    {
        private readonly IMemoryCache _cache;

        public CacheManager() => _cache = new MemoryCache(new MemoryCacheOptions());

        public T Get<T>(string key) where T : class => _cache.Get(key) as T;

        public void Remove(string key) => _cache.Remove(key);

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

            _cache.Set(key, instance, TimeService.UtcNow.Add(GetExpirationTime(duration, expiration)));
        }

        private static TimeSpan GetExpirationTime(CacheDuration duration, long expiration)
            => duration switch
            {
                CacheDuration.Hours => TimeSpan.FromHours(expiration),
                CacheDuration.Minutes => TimeSpan.FromMinutes(expiration),
                CacheDuration.Seconds => TimeSpan.FromSeconds(expiration),
                _ => throw new NotImplementedException($"Unknown {nameof(CacheDuration)}: {duration}"),
            };
    }
}
