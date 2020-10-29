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
        private const string ModesPath = "Modes";

        public static async Task<List<ModeDto>> GetAllModesAsync(
            HttpClient client,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            // Act
            var response = await client.GetAsync(ModesPath);

            // Assert
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
            // Act
            var response = await client.GetAsync($"{ModesPath}/{id}");

            // Assert
            Assert.AreEqual(expectedStatusCode, response.StatusCode);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ModeDto>(content);
        }

        public static async Task<int> CreateModesAsync(
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
            var result = await client.PostAsync(ModesPath, content);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);

            if (result.StatusCode != HttpStatusCode.OK)
            {
                await TestsHelper.AssertMessageOnBadRequestAsync(result, expectedMessageOnBadRequest);
                return -1;
            }

            var jsonString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(jsonString);
        }

        public static async Task<string> UpdateModesAsync(
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
            var result = await client.PutAsync($"{ModesPath}/{id}", content);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);

            if (result.StatusCode != HttpStatusCode.OK)
            {
                await TestsHelper.AssertMessageOnBadRequestAsync(result, expectedMessageOnBadRequest);
                return null;
            }

            return await result.Content.ReadAsStringAsync();
        }
    }
}
