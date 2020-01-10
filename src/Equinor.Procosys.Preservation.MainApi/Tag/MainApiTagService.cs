﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Exceptions;
using Equinor.Procosys.Preservation.MainApi.Plant;

namespace Equinor.Procosys.Preservation.MainApi.Tag
{
    public class MainApiTagService : ITagApiService
    {
        protected const string ApiVersion = "4.0";
        private readonly IMainApiClient _mainApiClient;
        private readonly IPlantApiService _plantApiService;

        public MainApiTagService(
            IMainApiClient mainApiClient,
            IPlantApiService plantApiService)
        {
            _mainApiClient = mainApiClient;
            _plantApiService = plantApiService;
        }

        public async Task<ProcosysTagDetails> GetTagDetails(string plant, string projectName, string tagNumber)
        {
            if (!await _plantApiService.IsPlantValidAsync(plant))
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
            var tagDetailsResult = await _mainApiClient.QueryAndDeserialize<ProcosysTagDetailsResult>(url);
            if (tagDetailsResult == null)
            {
                throw new InvalidResultException($"Tag details returned no data. URL: {url}");
            }
            return tagDetailsResult.Tag;
        }

        public async Task<IList<ProcosysTagOverview>> GetTags(string plant, string projectName, string startsWithTagNo) // TODO: Use paging to get all results
        {
            if (!await _plantApiService.IsPlantValidAsync(plant))
            {
                throw new ArgumentException($"Invalid plant: {plant}");
            }

            var url = $"Tag/Search?plantid={plant}&startsWithTagNo={startsWithTagNo}&projectName={projectName}&api-version={ApiVersion}";
            var tagSearchResult = await _mainApiClient.QueryAndDeserialize<ProcosysTagSearchResult>(url);
            if (tagSearchResult == null)
            {
                throw new InvalidResultException($"Tag search returned no data. URL: {url}");
            }
            return tagSearchResult.Items;
        }
    }
}
