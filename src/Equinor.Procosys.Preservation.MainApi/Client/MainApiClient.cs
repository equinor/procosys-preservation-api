using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.MainApi.Client
{
    public class MainApiClient : IMainApiClient
    {
        public static string Name => "MainApiClient";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBearerTokenProvider _bearerTokenProvider;
        private readonly MainApiOptions _options;
        private readonly ILogger<MainApiClient> _logger;

        public MainApiClient(
            IHttpClientFactory httpClientFactory,
            IBearerTokenProvider bearerTokenProvider,
            IOptionsMonitor<MainApiOptions> options,
            ILogger<MainApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _bearerTokenProvider = bearerTokenProvider;
            _options = options.CurrentValue;
            _logger = logger;
    }

        public async Task<T> QueryAndDeserialize<T>(string url)
        {
            var httpClient = _httpClientFactory.CreateClient(Name);
            httpClient.BaseAddress = new System.Uri(_options.BaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerTokenProvider.GetBearerToken());

            var stopWatch = Stopwatch.StartNew();
            var response = await httpClient.GetAsync(url);
            stopWatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"MainApi tag search was unsuccessful and took {stopWatch.Elapsed.TotalSeconds}s.");
                return default;
            }

            _logger.LogDebug($"MainApi tag search was successful and took {stopWatch.Elapsed.TotalSeconds}s.");
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(jsonResult);
            return result;
        }
    }
}
