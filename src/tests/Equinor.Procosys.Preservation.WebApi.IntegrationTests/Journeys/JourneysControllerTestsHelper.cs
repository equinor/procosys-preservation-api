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

        public static async Task<string> UpdateStepAsync(
            HttpClient client,
            int journeyId,
            int stepId,
            string title,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                title,
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
        
        public static async Task<StepDto> GetStepAsync(
            HttpClient client,
            int journeyId,
            int stepId,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var response = await client.GetAsync($"{_route}/{journeyId}/Steps/{stepId}");

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<StepDto>(content);
        }

        public static async Task<int> AddStepAsync(
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
            var response = await client.PostAsync($"{_route}/{journeyId}/Steps", content);
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
