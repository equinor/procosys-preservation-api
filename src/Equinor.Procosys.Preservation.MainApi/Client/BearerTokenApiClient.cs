using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.MainApi.Client
{
    public class BearerTokenApiClient : IBearerTokenApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBearerTokenProvider _bearerTokenProvider;
        private readonly ILogger<BearerTokenApiClient> _logger;

        public BearerTokenApiClient(
            IHttpClientFactory httpClientFactory,
            IBearerTokenProvider bearerTokenProvider,
            ILogger<BearerTokenApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _bearerTokenProvider = bearerTokenProvider;
            _logger = logger;
    }

        public async Task<T> QueryAndDeserialize<T>(string url)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerTokenProvider.GetBearerToken());

            var stopWatch = Stopwatch.StartNew();
            var response = await httpClient.GetAsync(url);
            stopWatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Request was unsuccessful and took {stopWatch.Elapsed.TotalSeconds}s.");
                return default;
            }

            _logger.LogDebug($"Request was successful and took {stopWatch.Elapsed.TotalSeconds}s.");
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(jsonResult);
            return result;
        }
    }
}
