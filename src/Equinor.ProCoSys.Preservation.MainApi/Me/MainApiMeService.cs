﻿using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Client;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.MainApi.Me
{
    public class MainApiMeService : IMeApiService
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IMainApiClientForUser _mainApiClient;

        public MainApiMeService(IMainApiClientForUser mainApiClient, IOptionsSnapshot<MainApiOptions> options)
        {
            _mainApiClient = mainApiClient;
            _apiVersion = options.Value.ApiVersion;
            _baseAddress = new Uri(options.Value.BaseAddress);
        }

        public async Task TracePlantAsync(string plant, CancellationToken cancellationToken)
        {
            var url = $"{_baseAddress}Me/TracePlant" +
                      $"?plantId={plant}" +
                      $"&api-version={_apiVersion}";

            var json = JsonSerializer.Serialize("ProCoSys - Preservation");
            await _mainApiClient.PostAsync(
                url,
                new StringContent(json, Encoding.Default, "application/json"),
                cancellationToken);
        }
    }
}
