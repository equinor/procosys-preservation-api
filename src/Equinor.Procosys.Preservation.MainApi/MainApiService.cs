using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Exceptions;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.MainApi
{
    public class MainApiService : ITagApiService, IPlantApiService
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

        private async Task<T> QueryAndDeserialize<T>(string url)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerTokenProvider.GetBearerToken());

            var stopWatch = Stopwatch.StartNew();
            var response = await _httpClient.GetAsync(url);
            stopWatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"MainApi tag search was unsuccessful and took {stopWatch.Elapsed.TotalSeconds}s.");
                return default;
            }

            _logger.LogDebug($"MainApi tag search was successful and took {stopWatch.Elapsed.TotalSeconds}s.");
            var jsonResult = await response.Content.ReadAsStringAsync();
            var tagSearchResult = JsonSerializer.Deserialize<T>(jsonResult);
            return tagSearchResult;
        }

        public async Task<ProcosysTagDetails> GetTagDetails(string plant, string projectName, string tagNumber)
        {
            if (!await IsPlantValidAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var tags = await GetTags(plant, projectName, tagNumber);
            if (tags.Count() != 1)
            {
                throw new InvalidResultException($"Expected 1, but was {tags.Count()}");
            }

            var tag = tags.First();
            var url = $"Tag?plantId=PCS${plant}&tagId={tag.Id}&api-version={ApiVersion}";
            var tagDetailsResult = await QueryAndDeserialize<ProcosysTagDetailsResult>(url);
            if (tagDetailsResult == null)
            {
                throw new InvalidResultException($"Tag details returned no data. URL: {url}");
            }
            return tagDetailsResult.Tag;
        }

        public async Task<IEnumerable<ProcosysTagOverview>> GetTags(string plant, string projectName, string startsWithTagNo) // TODO: Use paging to get all results
        {
            if (!await IsPlantValidAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var url = $"Tag/Search?plantid=PCS${plant}&startsWithTagNo={startsWithTagNo}&projectName={projectName}&api-version={ApiVersion}";
            var tagSearchResult = await QueryAndDeserialize<ProcosysTagSearchResult>(url);
            if (tagSearchResult == null)
            {
                throw new InvalidResultException($"Tag search returned no data. URL: {url}");
            }
            return tagSearchResult.Items;
        }

        public Task<IEnumerable<ProcosysPlant>> GetPlants() => QueryAndDeserialize<IEnumerable<ProcosysPlant>>($"Plants?api-version={ApiVersion}");

        public async Task<bool> IsPlantValidAsync(string plant)
        {
            var plants = await GetPlants();
            return plants.Any(p => p.Id == $"PCS${plant}");
        }
    }
}
