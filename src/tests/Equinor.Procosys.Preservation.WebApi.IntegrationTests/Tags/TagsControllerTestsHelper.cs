﻿using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using Newtonsoft.Json;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    public static class TagsControllerTestsHelper
    {
        private const string _route = "Tags";

        public static async Task<TagResultDto>  GetAllTagsAsync(
            HttpClient client,
            string projectName,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var parameters = new ParameterCollection {{"projectName", projectName}};
            var url = $"{_route}{parameters}";
            var response = await client.GetAsync(url);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TagResultDto>(content);
        }
        
        public static async Task<TagDetailsDto> GetTagAsync(
            HttpClient client,
            int id,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var response = await client.GetAsync($"{_route}/{id}");

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TagDetailsDto>(content);
        }

        public static async Task<int> DuplicateAreaTagAsync(
            HttpClient client,
            int sourceTagId,
            AreaTagType areaTagType,
            string disciplineCode,
            string areaCode,
            string tagNoSuffix,
            string description,
            string remark,
            string storageArea,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                sourceTagId,
                areaTagType,
                disciplineCode,
                areaCode,
                tagNoSuffix,
                description,
                remark,
                storageArea
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_route}/DuplicateArea", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return -1;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(jsonString);
        }
    }
}