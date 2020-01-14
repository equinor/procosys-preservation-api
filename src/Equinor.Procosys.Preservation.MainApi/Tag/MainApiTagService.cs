﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Exceptions;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.MainApi.Tag
{
    public class MainApiTagService : ITagApiService
    {
        private readonly string _apiVersion;
        private readonly IMainApiClient _mainApiClient;
        private readonly IPlantApiService _plantApiService;
        private readonly ILogger<MainApiTagService> _logger;

        public MainApiTagService(
            IMainApiClient mainApiClient,
            IPlantApiService plantApiService,
            IOptionsMonitor<MainApiOptions> options,
            ILogger<MainApiTagService> logger)
        {
            _mainApiClient = mainApiClient;
            _plantApiService = plantApiService;
            _apiVersion = options.CurrentValue.ApiVersion;
            _logger = logger;
        }

        public async Task<ProcosysTagDetails> GetTagDetails(string plant, string projectName, string tagNo)
        {
            if (!await _plantApiService.IsPlantValidAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var tags = await GetTags(plant, projectName, tagNo);
            if (tags.Count != 1)
            {
                throw new InvalidResultException($"Expected 1, but was {tags.Count()}");
            }

            var tag = tags.First();
            var url = $"Tag?plantId={plant}&tagId={tag.Id}&api-version={_apiVersion}";
            var tagDetailsResult = await _mainApiClient.QueryAndDeserialize<ProcosysTagDetailsResult>(url);
            if (tagDetailsResult == null)
            {
                _logger.LogWarning($"Tag details returned no data. URL: {url}");
                return default;
            }
            return tagDetailsResult.Tag;
        }

        public async Task<IList<ProcosysTagOverview>> GetTags(string plant, string projectName, string startsWithTagNo) // TODO: Use paging to get all results
        {
            if (!await _plantApiService.IsPlantValidAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var url = $"Tag/Search?plantid={plant}&startsWithTagNo={startsWithTagNo}&projectName={projectName}&api-version={_apiVersion}";
            var tagSearchResult = await _mainApiClient.QueryAndDeserialize<ProcosysTagSearchResult>(url);
            if (tagSearchResult == null)
            {
                _logger.LogWarning($"Tag search returned no data. URL: {url}");
                return default;
            }
            return tagSearchResult.Items;
        }
    }
}
