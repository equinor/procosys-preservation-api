using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Client;
using Equinor.ProCoSys.Preservation.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Tag
{
    public class MainApiTagService : ITagApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IMainApiClientForUser _mainApiClient;
        private readonly int _tagSearchPageSize;

        public MainApiTagService(
            IMainApiClientForUser mainApiClient,
            IOptionsMonitor<MainApiOptions> mainApiOptions,
            IOptionsMonitor<TagOptions> tagOptions,
            ILogger<MainApiTagService> logger)
        {
            _mainApiClient = mainApiClient;
            _apiVersion = mainApiOptions.CurrentValue.ApiVersion;
            _baseAddress = new Uri(mainApiOptions.CurrentValue.BaseAddress);
            if (tagOptions.CurrentValue.TagSearchPageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tagOptions.CurrentValue.TagSearchPageSize), "Must be a positive number.");
            }
            _tagSearchPageSize = tagOptions.CurrentValue.TagSearchPageSize;
            if (_tagSearchPageSize < 100)
            {
                logger.LogWarning("Tag search page size is set to a low value. This may impact the overall performance!");
            }
        }

        public async Task<IList<PCSTagDetails>> GetTagDetailsAsync(
            string plant,
            string projectName,
            IList<string> allTagNos,
            CancellationToken cancellationToken,
            bool includeVoidedTags = false)
        {
            if (allTagNos == null)
            {
                throw new ArgumentNullException(nameof(allTagNos));
            }

            var baseUrl = $"{_baseAddress}Tag/ByTagNos" +
                $"?plantId={plant}" +
                $"&projectName={WebUtility.UrlEncode(projectName)}" +
                $"&includeVoidedTags={includeVoidedTags.ToString().ToLower()}" + 
                $"&api-version={_apiVersion}";

            var tagDetails = new List<PCSTagDetails>();
            var page = 0;
            // Use relative small page size since TagNos are added to querystring of url and maxlength is 2000
            var pageSize = 50;
            IEnumerable<string> pageWithTagNos;
            do
            {
                pageWithTagNos = allTagNos.Skip(pageSize * page).Take(pageSize).ToList();

                if (pageWithTagNos.Any())
                {
                    var url = baseUrl;
                    foreach (var tagNo in pageWithTagNos)
                    {
                        url += $"&tagNos={WebUtility.UrlEncode(tagNo)}";
                    }

                    var tagDetailsPage = await _mainApiClient.QueryAndDeserializeAsync<List<PCSTagDetails>>(url, cancellationToken);
                    tagDetails.AddRange(tagDetailsPage);
                }

                page++;

            } while (pageWithTagNos.Count() == pageSize);
            return tagDetails;
        }

        public async Task<IList<PCSPreservedTag>> GetPreservedTagsAsync(
            string plant,
            string projectName,
            CancellationToken cancellationToken)
        {
            var url = $"{_baseAddress}PreservationTags" +
                      $"?plantId={plant}" +
                      $"&projectName={WebUtility.UrlEncode(projectName)}" +
                      $"&api-version={_apiVersion}";
            return await _mainApiClient.QueryAndDeserializeAsync<List<PCSPreservedTag>>(url, cancellationToken);
        }

        public async Task<IList<PCSTagOverview>> SearchTagsByTagNoAsync(string plant, string projectName, string startsWithTagNo, CancellationToken cancellationToken)
        {
            var items = new List<PCSTagOverview>();
            var currentPage = 0;
            PCSTagSearchResult tagSearchResult;
            do
            {
                var url = $"{_baseAddress}Tag/Search" +
                    $"?plantId={plant}" +
                    $"&startsWithTagNo={WebUtility.UrlEncode(startsWithTagNo)}" +
                    $"&projectName={WebUtility.UrlEncode(projectName)}" +
                    $"&currentPage={currentPage++}" +
                    $"&itemsPerPage={_tagSearchPageSize}" +
                    $"&calculateMccrResponsibleCodes=true" +
                    $"&api-version={_apiVersion}";
                tagSearchResult = await _mainApiClient.QueryAndDeserializeAsync<PCSTagSearchResult>(url, cancellationToken);
                if (tagSearchResult?.Items != null && tagSearchResult.Items.Any())
                {
                    items.AddRange(tagSearchResult.Items);
                }
            } while (tagSearchResult != null && items.Count < tagSearchResult.MaxAvailable);
            return items;
        }

        public async Task<IList<PCSTagOverview>> SearchTagsByTagFunctionsAsync(string plant, string projectName, IList<string> tagFunctionCodeRegisterCodePairs, CancellationToken cancellationToken)
        {
            var items = new List<PCSTagOverview>();
            var currentPage = 0;
            PCSTagSearchResult tagSearchResult;
            do
            {
                var url = $"{_baseAddress}Tag/Search/ByTagFunctions" +
                          $"?plantId={plant}" +
                          $"&projectName={WebUtility.UrlEncode(projectName)}" +
                          $"&currentPage={currentPage++}" +
                          $"&itemsPerPage={_tagSearchPageSize}" +
                          $"&api-version={_apiVersion}";
                foreach (var tagFunctionCodeRegisterCodePair in tagFunctionCodeRegisterCodePairs)
                {
                    url += $"&tagFunctionCodeRegisterCodePairs={tagFunctionCodeRegisterCodePair}";
                }

                tagSearchResult = await _mainApiClient.QueryAndDeserializeAsync<PCSTagSearchResult>(url, cancellationToken);
                if (tagSearchResult?.Items != null && tagSearchResult.Items.Any())
                {
                    items.AddRange(tagSearchResult.Items);
                }
            } while (tagSearchResult != null && items.Count < tagSearchResult.MaxAvailable);
            return items;
        }

        public async Task MarkTagsAsMigratedAsync(string plant, IEnumerable<long> tagIds, CancellationToken cancellationToken)
        {
            var url = $"{_baseAddress}PreservationTags" +
                      $"?plantId={plant}" +
                      $"&api-version={_apiVersion}";

            var json = JsonSerializer.Serialize(tagIds);
            await _mainApiClient.PutAsync(
                url,
                new StringContent(json, Encoding.Default, "application/json"),
                cancellationToken);
        }
    }
}
