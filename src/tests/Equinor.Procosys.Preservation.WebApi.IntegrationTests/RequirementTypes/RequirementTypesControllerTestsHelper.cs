using System.Collections.Generic;
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

        public static async Task<string> UpdateRequirementDefinitionAsync(
            HttpClient client,
            int requirementTypeId,
            int requirementDefinitionId,
            string title,
            int defaultIntervalWeeks,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                title,
                defaultIntervalWeeks,
                sortKey = 10,
                rowVersion
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_route}/{requirementTypeId}/RequirementDefinitions/{requirementDefinitionId}", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
