using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Infrastructure.Caching;
using Equinor.ProCoSys.Preservation.MainApi.Person;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Caches
{
    public class PersonCache : IPersonCache
    {
        private readonly ICacheManager _cacheManager;
        private readonly IPersonApiService _personApiService;
        private readonly IOptionsSnapshot<CacheOptions> _options;

        public PersonCache(
            ICacheManager cacheManager, 
            ICurrentUserProvider currentUserProvider, 
            IPersonApiService personApiService,
            IOptionsSnapshot<CacheOptions> options)
        {
            _cacheManager = cacheManager;
            _personApiService = personApiService;
            _options = options;
        }

        public async Task<PCSPerson> GetAsync(Guid userOid)
            => await _cacheManager.GetOrCreate(
                PersonsCacheKey(userOid),
                async () =>
                {
                    var person = await _personApiService.TryGetPersonByOidAsync(userOid);
                    return person;
                },
                CacheDuration.Minutes,
                _options.Value.PersonCacheMinutes);

        public async Task<bool> ExistsAsync(Guid userOid)
        {
            var pcsPerson = await GetAsync(userOid);
            return pcsPerson != null;
        }

        private string PersonsCacheKey(Guid userOid)
            => $"PERSONS_{userOid.ToString().ToUpper()}";
    }
}
