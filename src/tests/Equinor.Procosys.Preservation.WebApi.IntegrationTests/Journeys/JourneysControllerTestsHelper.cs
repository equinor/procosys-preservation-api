using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Journeys
{
    public static class JourneysControllerTestsHelper
    {
        private const string _route = "Journeys";
        
        public static async Task<List<JourneyDto>> GetJourneysAsync(
            HttpClient client,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var parameters = new ParameterCollection {{"includeVoided", "true"}};
            var url = $"{_route}{parameters}";
            var response = await client.GetAsync(url);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<JourneyDto>>(content);
        }
        
        public static async Task<JourneyDetailsDto> GetJourneyAsync(
            HttpClient client,
            int journeyId,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var parameters = new ParameterCollection {{"includeVoided", "true"}};
            var url = $"{_route}/{journeyId}{parameters}";
            var response = await client.GetAsync(url);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<JourneyDetailsDto>(content);
        }

        public static async Task<int> CreateStepAsync(
            HttpClient client,
            int journeyId,
            string title,
            int modeId,
            string responsibleCode,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                title,
                modeId,
                responsibleCode
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_route}/{journeyId}/AddStep", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return -1;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(jsonString);
        }

        public static async Task<string> UpdateStepAsync(
            HttpClient client,
            int journeyId,
            int stepId,
            string title,
            int modeId,
            string responsibleCode,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                title,
                modeId,
                responsibleCode,
                rowVersion
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_route}/{journeyId}/Steps/{stepId}", content);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> VoidStepAsync(
            HttpClient client,
            int journeyId,
            int stepId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
            => await VoidUnvoidStepAsync(client,
                journeyId,
                stepId,
                rowVersion,
                "Void",
                expectedStatusCode,
                expectedMessageOnBadRequest);

        public static async Task<string> UnvoidStepAsync(
            HttpClient client,
            int journeyId,
            int stepId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
            => await VoidUnvoidStepAsync(client,
                journeyId,
                stepId,
                rowVersion,
                "Unvoid",
                expectedStatusCode,
                expectedMessageOnBadRequest);

        private static async Task<string> VoidUnvoidStepAsync(
            HttpClient client,
            int journeyId,
            int stepId,
            string rowVersion,
            string action,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                rowVersion
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_route}/{journeyId}/Steps/{stepId}/{action}", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
