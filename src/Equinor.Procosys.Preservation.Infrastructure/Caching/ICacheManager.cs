using System;

namespace Equinor.Procosys.Preservation.Infrastructure.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key) where T : class;
        T GetOrCreate<T>(string key, Func<T> fetch, CacheDuration duration, long expiration) where T : class;
    }
}
