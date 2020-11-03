using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    public static class TagsControllerTestsHelper
    {
        private const string _route = "Tags";

        public static async Task<TagResultDto>  GetAllTagsAsync(
            HttpClient client,
            string projectName,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var parameters = new ParameterCollection {{"projectName", projectName}};
            var url = $"{_route}{parameters}";
            var response = await client.GetAsync(url);

            Assert.AreEqual(expectedStatusCode, response.StatusCode);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TagResultDto>(content);
        }
    }
}
