using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Person
{
    public class MainApiPersonService : IPersonApiService
    {
        private readonly Uri _baseAddress;
        private readonly string _apiVersion;
        private readonly IBearerTokenApiClient _mainApiClient;

        public MainApiPersonService(
            IBearerTokenApiClient mainApiClient,
            IOptionsMonitor<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
            _apiVersion = options.CurrentValue.ApiVersion;
        }

        public async Task<IList<PCSPerson>> GetPersonsByOidsAsync(string plant, IList<string> azureOids)
        {
            var url = $"{_baseAddress}Person/PersonsByOids" +
                      $"?plantId={plant}" +
                      $"&api-version={_apiVersion}";
            foreach (var oid in azureOids)
            {
                url += $"&azureOids={oid}";
            }

            return await _mainApiClient.QueryAndDeserializeAsync<List<PCSPerson>>(url);
        }
    }
}
