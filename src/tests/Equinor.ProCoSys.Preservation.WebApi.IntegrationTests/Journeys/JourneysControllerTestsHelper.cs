using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Journeys
{
    public static class JourneysControllerTestsHelper
    {
        private const string _route = "Journeys";

        public static async Task<int> CreateJourneyAsync(
            UserType userType,
            string plant,
            string title,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                title
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await TestFactory.Instance.GetHttpClient(userType, plant).PostAsync($"{_route}", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return -1;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(jsonString);
        }

        public static async Task<List<JourneyDto>> GetJourneysAsync(
            UserType userType,
            string plant,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var parameters = new ParameterCollection { { "includeVoided", "true" } };
            var url = $"{_route}{parameters}";
            var response = await TestFactory.Instance.GetHttpClient(userType, plant).GetAsync(url);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<JourneyDto>>(jsonString);
        }

        public static async Task<JourneyDetailsDto> GetJourneyAsync(
            UserType userType,
            string plant,
            int journeyId,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var parameters = new ParameterCollection { { "includeVoided", "true" } };
            var url = $"{_route}/{journeyId}{parameters}";
            var response = await TestFactory.Instance.GetHttpClient(userType, plant).GetAsync(url);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<JourneyDetailsDto>(jsonString);
        }

        public static async Task<string> UpdateJourneyAsync(
            UserType userType,
            string plant,
            int journeyId,
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
            var response = await TestFactory.Instance.GetHttpClient(userType, plant).PutAsync($"{_route}/{journeyId}", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> VoidJourneyAsync(
            UserType userType,
            string plant,
            int journeyId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
            => await VoidUnvoidJourneyAsync(TestFactory.Instance.GetHttpClient(userType, plant),
                journeyId,
                rowVersion,
                "Void",
                expectedStatusCode,
                expectedMessageOnBadRequest);

        public static async Task<string> UnvoidJourneyAsync(
            UserType userType,
            string plant,
            int journeyId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
            => await VoidUnvoidJourneyAsync(TestFactory.Instance.GetHttpClient(userType, plant),
                journeyId,
                rowVersion,
                "Unvoid",
                expectedStatusCode,
                expectedMessageOnBadRequest);

        public static async Task DeleteJourneyAsync(
            UserType userType,
            string plant,
            int journeyId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                rowVersion
            };
            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_route}/{journeyId}")
            {
                Content = new StringContent(serializePayload, Encoding.UTF8, "application/json")
            };

            var response = await TestFactory.Instance.GetHttpClient(userType, plant).SendAsync(request);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);
        }

        public static async Task<int> CreateStepAsync(
            UserType userType,
            string plant,
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
            var response = await TestFactory.Instance.GetHttpClient(userType, plant).PostAsync($"{_route}/{journeyId}/AddStep", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return -1;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(jsonString);
        }

        public static async Task<string> UpdateStepAsync(
            UserType userType,
            string plant,
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
            var response = await TestFactory.Instance.GetHttpClient(userType, plant).PutAsync($"{_route}/{journeyId}/Steps/{stepId}", content);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> VoidStepAsync(
            UserType userType,
            string plant,
            int journeyId,
            int stepId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
            => await VoidUnvoidStepAsync(TestFactory.Instance.GetHttpClient(userType, plant),
                journeyId,
                stepId,
                rowVersion,
                "Void",
                expectedStatusCode,
                expectedMessageOnBadRequest);

        public static async Task<string> UnvoidStepAsync(
            UserType userType,
            string plant,
            int journeyId,
            int stepId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
            => await VoidUnvoidStepAsync(TestFactory.Instance.GetHttpClient(userType, plant),
                journeyId,
                stepId,
                rowVersion,
                "Unvoid",
                expectedStatusCode,
                expectedMessageOnBadRequest);

        public static async Task DeleteStepAsync(
            UserType userType,
            string plant,
            int journeyId,
            int stepId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                rowVersion
            };
            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_route}/{journeyId}/Steps/{stepId}")
            {
                Content = new StringContent(serializePayload, Encoding.UTF8, "application/json")
            };

            var response = await TestFactory.Instance.GetHttpClient(userType, plant).SendAsync(request);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);
        }

        public static async Task<List<StepIdAndRowVersion>> SwapStepsAsync(
            UserType userType,
            string plant,
            int journeyId,
            StepIdAndRowVersion stepA,
            StepIdAndRowVersion stepB,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                stepA,
                stepB
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await TestFactory.Instance.GetHttpClient(userType, plant).PutAsync($"{_route}/{journeyId}/Steps/SwapSteps", content);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<StepIdAndRowVersion>>(jsonString);

        }

        private static async Task<string> VoidUnvoidJourneyAsync(
            HttpClient client,
            int journeyId,
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
            var response = await client.PutAsync($"{_route}/{journeyId}/{action}", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

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
