using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.TagFunctions
{
    public static class TagFunctionsControllerTestsHelper
    {
        private const string _route = "TagFunctions";
        
        public static async Task<TagFunctionDetailsDto> GetTagFunctionDetailsAsync(
            UserType userType,
            string plant,
            string code,
            string registerCode,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var parameters = new ParameterCollection
            {
                {"registerCode", registerCode}
            };
            var url = $"{_route}/{code}{parameters}";
            var response = await TestFactory.Instance.GetHttpClient(userType, plant).GetAsync(url);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TagFunctionDetailsDto>(content);
        }

        public static async Task UpdateTagFunctionAsync(
            UserType userType,
            string plant,
            string tagFunctionCode,
            string registerCode,
            IEnumerable<TagFunctionRequirementDto> requirements,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                tagFunctionCode,
                registerCode,
                requirements
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await TestFactory.Instance.GetHttpClient(userType, plant).PutAsync(_route, content);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);
        }

        public static async Task<string> VoidTagFunctionAsync(
            UserType userType,
            string plant,
            string code,
            string registerCode,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
            => await VoidUnvoidTagFunctionAsync(
                TestFactory.Instance.GetHttpClient(userType, plant),
                code,
                registerCode,
                rowVersion,
                "Void",
                expectedStatusCode,
                expectedMessageOnBadRequest);

        public static async Task<string> UnvoidTagFunctionAsync(
            UserType userType,
            string plant,
            string code,
            string registerCode,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
            => await VoidUnvoidTagFunctionAsync(
                TestFactory.Instance.GetHttpClient(userType, plant),
                code,
                registerCode,
                rowVersion,
                "Unvoid",
                expectedStatusCode,
                expectedMessageOnBadRequest);

        private static async Task<string> VoidUnvoidTagFunctionAsync(
            HttpClient client,
            string code,
            string registerCode,
            string rowVersion,
            string action,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                registerCode,
                rowVersion
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_route}/{code}/{action}", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
