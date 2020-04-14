using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IPlantApiService _plantApiService;
        private readonly ILogger<MainApiTagService> _logger;
        private readonly int _tagSearchPageSize;

        public MainApiTagService(
            IBearerTokenApiClient mainApiClient,
            IPlantApiService plantApiService,
            IOptionsMonitor<MainApiOptions> options,
            ILogger<MainApiTagService> logger)
        {
            _mainApiClient = mainApiClient;
            _plantApiService = plantApiService;
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
        }

        public async Task<IList<ProcosysTagDetails>> GetTagDetailsAsync(string plant, string projectName, IEnumerable<string> tagNos)
        {
            if (tagNos == null)
            {
                throw new ArgumentNullException(nameof(tagNos));

            }
            if (!await _plantApiService.IsPlantValidAsync(plant))
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

            var tagDetails = await _mainApiClient.QueryAndDeserialize<List<ProcosysTagDetails>>(url);
            
            if (tagDetails == null)
            {
                _logger.LogWarning($"Tag details returned no data. URL: {url}");
                return default;
            }
            return tagDetails;
        }

        public async Task<IList<ProcosysTagOverview>> SearchTagsByTagNoAsync(string plant, string projectName, string startsWithTagNo)
        {
            if (!await _plantApiService.IsPlantValidAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var items = new List<ProcosysTagOverview>();
            var currentPage = 0;
            do
            {
                var url = $"{_baseAddress}Tag/Search" +
                    $"?plantId={plant}" +
                    $"&startsWithTagNo={startsWithTagNo}" +
                    $"&projectName={projectName}" +
                    $"&currentPage={currentPage++}" +
                    $"&itemsPerPage={_tagSearchPageSize}" +
                    $"&api-version={_apiVersion}";
                var tagSearchResult = await _mainApiClient.QueryAndDeserialize<ProcosysTagSearchResult>(url);
                if (tagSearchResult?.Items != null && tagSearchResult.Items.Any())
                {
                    items.AddRange(tagSearchResult.Items);
                }
                else
                {
                    return items;
                }
            } while (true);
        }

        public async Task<IList<ProcosysTagOverview>> SearchTagsByTagFunctionAsync(string plant, string projectName, string tagFunctionCode, string registerCode)
        {
            if (!await _plantApiService.IsPlantValidAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var items = new List<ProcosysTagOverview>();
            var currentPage = 0;
            do
            {
                var url = $"{_baseAddress}Tag/Search/ByTagFunction" +
                          $"?plantId={plant}" +
                          $"&projectName={projectName}" +
                          $"&tagFunctionCode={tagFunctionCode}" +
                          $"&registerCode={registerCode}" +
                          $"&currentPage={currentPage++}" +
                          $"&itemsPerPage={_tagSearchPageSize}" +
                          $"&api-version={_apiVersion}";

                var tagSearchResult = await _mainApiClient.QueryAndDeserialize<ProcosysTagSearchResult>(url);
                if (tagSearchResult?.Items != null && tagSearchResult.Items.Any())
                {
                    items.AddRange(tagSearchResult.Items);
                }
                else
                {
                    return items;
                }
            } while (true);
        }
    }
}
