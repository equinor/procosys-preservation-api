using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.MainApi
{
    public class MainApiService : ITagApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IBearerTokenProvider _bearerTokenProvider;
        private readonly ILogger<MainApiService> _logger;
        private const string ApiVersion = "4.0";

        public MainApiService(
            HttpClient httpClient,
            IBearerTokenProvider bearerTokenProvider,
            ILogger<MainApiService> logger)
        {
            _httpClient = httpClient;
            _bearerTokenProvider = bearerTokenProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<MainTagDto>> GetTags(string plant, string startsWithTagNo) // TODO: Use paging to get all results
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerTokenProvider.GetBearerToken());

            var stopWatch = Stopwatch.StartNew();
            var response = await _httpClient.GetAsync($"Tag/Search?plantid=PCS${plant}&startsWithTagNo={startsWithTagNo}&api-version={ApiVersion}");
            stopWatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"MainApi tag search was unsuccessful and took {stopWatch.Elapsed.TotalSeconds}s.");
                return new List<MainTagDto>();
            }

            _logger.LogDebug($"MainApi tag search was successful and took {stopWatch.Elapsed.TotalSeconds}s.");
            var jsonResult = await response.Content.ReadAsStringAsync();
            var tagSearchResult = JsonSerializer.Deserialize<MainTagSearchDto>(jsonResult);
            return tagSearchResult.Items;
        }
    }
}
