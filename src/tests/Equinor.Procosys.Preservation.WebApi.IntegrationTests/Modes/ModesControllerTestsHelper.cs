using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Modes
{
    public static class ModesControllerTestsHelper
    {
        private const string _route = "Modes";

        public static async Task<List<ModeDto>> GetAllModesAsync(
            HttpClient client,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var response = await client.GetAsync(_route);

            Assert.AreEqual(expectedStatusCode, response.StatusCode);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ModeDto>>(content);
        }
        
        public static async Task<ModeDto> GetModeAsync(
            HttpClient client,
            int id,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var response = await client.GetAsync($"{_route}/{id}");

            Assert.AreEqual(expectedStatusCode, response.StatusCode);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ModeDto>(content);
        }

        public static async Task<int> CreateModeAsync(
            HttpClient client,
            string title,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                title,
                forSupplier = false
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(_route, content);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);

            if (result.StatusCode != HttpStatusCode.OK)
            {
                await TestsHelper.AssertMessageOnBadRequestAsync(result, expectedMessageOnBadRequest);
                return -1;
            }

            var jsonString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(jsonString);
        }

        public static async Task<string> UpdateModeAsync(
            HttpClient client,
            int id,
            string title,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                title,
                forSupplier = false,
                rowVersion
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var result = await client.PutAsync($"{_route}/{id}", content);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);

            if (result.StatusCode != HttpStatusCode.OK)
            {
                await TestsHelper.AssertMessageOnBadRequestAsync(result, expectedMessageOnBadRequest);
                return null;
            }

            return await result.Content.ReadAsStringAsync();
        }

        public static async Task DeleteModeAsync(
            HttpClient client,
            int id,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                rowVersion
            };
            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_route}/{id}")
            {
                Content = new StringContent(serializePayload, Encoding.UTF8, "application/json")
            };

            var result = await client.SendAsync(request);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);

            await TestsHelper.AssertMessageOnBadRequestAsync(result, expectedMessageOnBadRequest);
        }
    }
}
