using System;

namespace Equinor.Procosys.Preservation.Infrastructure.Caching
{
    public interface ICacheManager
    {
        T GetOrCreate<T>(string key, Func<T> fetch, CacheDuration duration, long expiration) where T : class;
    }
}
