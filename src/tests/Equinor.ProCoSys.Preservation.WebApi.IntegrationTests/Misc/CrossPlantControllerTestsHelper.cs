using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Misc
{
    public static class CrossPlantControllerTestsHelper
    {
        private const string _route = "CrossPlant";

        public static async Task<List<ActionDto>> GetActionsAsync(
            UserType userType,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var response = await TestFactory.Instance.GetHttpClient(userType, null).GetAsync($"{_route}/Actions");

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ActionDto>>(jsonString);
        }

        public static async Task<List<TagDto>> GetTagsAsync(
            UserType userType,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var response = await TestFactory.Instance.GetHttpClient(userType, null).GetAsync($"{_route}/Tags");

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TagDto>>(jsonString);
        }
    }
}
