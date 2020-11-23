using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.RequirementTypes
{
    public static class RequirementTypesControllerTestsHelper
    {
        private const string _route = "RequirementTypes";

        public static async Task<List<RequirementTypeDto>> GetRequirementTypesAsync(
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
            return JsonConvert.DeserializeObject<List<RequirementTypeDto>>(content);
        }
        
        public static async Task<int> CreateRequirementDefinitionAsync(
            HttpClient client,
            int reqTypeId,
            string title,
            List<FieldDto> fields = null,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                title,
                sortKey = 10,
                defaultIntervalWeeks = 4,
                fields
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_route}/{reqTypeId}/RequirementDefinitions", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return -1;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(jsonString);
        }

        public static async Task<string> UpdateRequirementDefinitionAsync(
            HttpClient client,
            int requirementTypeId,
            int requirementDefinitionId,
            string title,
            int defaultIntervalWeeks,
            string rowVersion,
            List<FieldDetailsDto> updatedFields = null,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                title,
                defaultIntervalWeeks,
                sortKey = 10,
                rowVersion,
                updatedFields
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response =
                await client.PutAsync($"{_route}/{requirementTypeId}/RequirementDefinitions/{requirementDefinitionId}",
                    content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> VoidRequirementDefinitionAsync(
            HttpClient client,
            int reqTypeId,
            int reqDefId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
            => await VoidUnvoidRequirementDefinitionAsync(client,
                reqTypeId,
                reqDefId,
                rowVersion,
                "Void",
                expectedStatusCode,
                expectedMessageOnBadRequest);

        public static async Task<string> UnvoidRequirementDefinitionAsync(
            HttpClient client,
            int reqTypeId,
            int reqDefId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
            => await VoidUnvoidRequirementDefinitionAsync(client,
                reqTypeId,
                reqDefId,
                rowVersion,
                "Unvoid",
                expectedStatusCode,
                expectedMessageOnBadRequest);

        public static async Task DeleteRequirementDefinitionAsync(
            HttpClient client,
            int reqTypeId,
            int reqDefId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                rowVersion
            };
            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_route}/{reqTypeId}/RequirementDefinitions/{reqDefId}")
            {
                Content = new StringContent(serializePayload, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);
        }
        
        public static async Task<RequirementDefinitionDto> GetRequirementDefinitionDetailsAsync(HttpClient client, int reqTypeId, int reqDefId)
        {
            var reqType = await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(client);
            return reqType
                .Single(r => r.Id == reqTypeId)
                .RequirementDefinitions
                .SingleOrDefault(s => s.Id == reqDefId);
        }

        private static async Task<string> VoidUnvoidRequirementDefinitionAsync(
            HttpClient client,
            int reqTypeId,
            int reqDefId,
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
            var response = await client.PutAsync($"{_route}/{reqTypeId}/RequirementDefinitions/{reqDefId}/{action}", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
