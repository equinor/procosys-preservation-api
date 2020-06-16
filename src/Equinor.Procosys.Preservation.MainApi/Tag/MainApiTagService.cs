using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.MainApi.Tag
{
    public class MainApiTagService : ITagApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;
        private readonly IPlantCache _plantCache;
        private readonly ILogger<MainApiTagService> _logger;
        private readonly int _tagSearchPageSize;

        public MainApiTagService(
            IBearerTokenApiClient mainApiClient,
            IPlantCache plantCache,
            IOptionsMonitor<MainApiOptions> options,
            ILogger<MainApiTagService> logger)
        {
            _mainApiClient = mainApiClient;
            _plantCache = plantCache;
            _logger = logger;
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
            if (options.CurrentValue.TagSearchPageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(options.CurrentValue.TagSearchPageSize), "Must be a positive number.");
            }
            _tagSearchPageSize = options.CurrentValue.TagSearchPageSize;
            if (_tagSearchPageSize < 100)
            {
                _logger.LogWarning("Tag search page size is set to a low value. This may impact the overall performance!");
            }
            if (_tagSearchPageSize <= 0)
            {
                throw new Exception($"{nameof(options.CurrentValue.TagSearchPageSize)} can't be zero or negative");
            }
        }

        public async Task<IList<ProcosysTagDetails>> GetTagDetailsAsync(string plant, string projectName, IEnumerable<string> tagNos)
        {
            if (tagNos == null)
            {
                throw new ArgumentNullException(nameof(tagNos));

            }
            if (!await _plantCache.IsValidPlantForCurrentUserAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var url = $"{_baseAddress}Tag/ByTagNos" +
                $"?plantId={plant}" +
                $"&projectName={projectName}" +
                $"&api-version={_apiVersion}";
            foreach (var tagNo in tagNos)
            {
                url += $"&tagNos={tagNo}";
            }

            var tagDetails = await _mainApiClient.QueryAndDeserializeAsync<List<ProcosysTagDetails>>(url);
            
            if (tagDetails == null)
            {
                _logger.LogWarning($"Tag details returned no data. URL: {url}");
                return default;
            }
            return tagDetails;
        }

        public async Task<IList<ProcosysPreservedTag>> GetPreservedTagsAsync(string plant, string projectName)
        {
            if (!await _plantCache.IsValidPlantForCurrentUserAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var url = $"{_baseAddress}PreservationTags" +
                      $"?plantId={plant}" +
                      $"&projectName={projectName}" +
                      $"&api-version={_apiVersion}";
            return await _mainApiClient.QueryAndDeserializeAsync<List<ProcosysPreservedTag>>(url);
        }

        public async Task<IList<ProcosysTagOverview>> SearchTagsByTagNoAsync(string plant, string projectName, string startsWithTagNo)
        {
            if (!await _plantCache.IsValidPlantForCurrentUserAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var items = new List<ProcosysTagOverview>();
            var currentPage = 0;
            ProcosysTagSearchResult tagSearchResult;
            do
            {
                var url = $"{_baseAddress}Tag/Search" +
                    $"?plantId={plant}" +
                    $"&startsWithTagNo={startsWithTagNo}" +
                    $"&projectName={projectName}" +
                    $"&currentPage={currentPage++}" +
                    $"&itemsPerPage={_tagSearchPageSize}" +
                    $"&api-version={_apiVersion}";
                tagSearchResult = await _mainApiClient.QueryAndDeserializeAsync<ProcosysTagSearchResult>(url);
                if (tagSearchResult?.Items != null && tagSearchResult.Items.Any())
                {
                    items.AddRange(tagSearchResult.Items);
                }
            } while (tagSearchResult != null && items.Count < tagSearchResult.MaxAvailable);
            return items;
        }

        public async Task<IList<ProcosysTagOverview>> SearchTagsByTagFunctionsAsync(string plant, string projectName, IList<string> tagFunctionCodeRegisterCodePairs)
        {
            if (!await _plantCache.IsValidPlantForCurrentUserAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var items = new List<ProcosysTagOverview>();
            var currentPage = 0;
            ProcosysTagSearchResult tagSearchResult;
            do
            {
                var url = $"{_baseAddress}Tag/Search/ByTagFunction" +
                          $"?plantId={plant}" +
                          $"&projectName={projectName}" +
                          $"&currentPage={currentPage++}" +
                          $"&itemsPerPage={_tagSearchPageSize}" +
                          $"&api-version={_apiVersion}";
                foreach (var tagFunctionCodeRegisterCodePair in tagFunctionCodeRegisterCodePairs)
                {
                    url += $"&tagFunctionCodeRegisterCodePairs={tagFunctionCodeRegisterCodePair}";
                }

                tagSearchResult = await _mainApiClient.QueryAndDeserializeAsync<ProcosysTagSearchResult>(url);
                if (tagSearchResult?.Items != null && tagSearchResult.Items.Any())
                {
                    items.AddRange(tagSearchResult.Items);
                }
            } while (tagSearchResult != null && items.Count < tagSearchResult.MaxAvailable);
            return items;
        }

        public async Task MarkTagsAsMigratedAsync(string plant, IEnumerable<long> tagIds)
        {
            if (!await _plantCache.IsValidPlantForCurrentUserAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var url = $"{_baseAddress}PreservationTags" +
                      $"?plantId={plant}" +
                      $"&api-version={_apiVersion}";

            var json = JsonSerializer.Serialize(tagIds);
            await _mainApiClient.PutAsync(url, new StringContent(json, Encoding.Default, "application/json"));
        }
    }
}
