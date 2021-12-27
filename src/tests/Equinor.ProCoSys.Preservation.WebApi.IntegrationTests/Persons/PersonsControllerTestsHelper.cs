using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Persons
{
    public static class PersonsControllerTestsHelper
    {
        private const string _route = "Persons";

        public static async Task<SavedFilterDto> CreateAndGetFilterAsync(
            UserType userType,
            string plant,
            string projectName,
            string title)
        {
            var id = await CreateSavedFilterAsync(
                userType,
                plant,
                projectName,
                title,
                Guid.NewGuid().ToString(),
                false);

            // Assert
            Assert.IsTrue(id > 0);
            var filters = await GetSavedFiltersInProjectAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);
            var filter = filters.SingleOrDefault(f => f.Id == id);
            Assert.IsNotNull(filter);
            return filter;

        }

        public static async Task<List<SavedFilterDto>> GetSavedFiltersInProjectAsync(
            UserType userType,
            string plant,
            string projectName,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var parameters = new ParameterCollection
            {
                {"projectName", projectName}
            };
            var url = $"{_route}/SavedFilters{parameters}";
            var response = await TestFactory.Instance.GetHttpClient(userType, plant).GetAsync(url);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<SavedFilterDto>>(content);
        }

        public static async Task<int> CreateSavedFilterAsync(
            UserType userType,
            string plant,
            string projectName,
            string title,
            string criteria,
            bool defaultFilter,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                projectName,
                title,
                criteria,
                defaultFilter
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await TestFactory.Instance.GetHttpClient(userType, plant).PostAsync($"{_route}/SavedFilter", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return -1;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(jsonString);
        }

        public static async Task<string> UpdateSavedFilterAsync(
            UserType userType,
            string plant,
            int id,
            string title,
            string criteria,
            bool defaultFilter,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                title,
                criteria,
                defaultFilter,
                rowVersion
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await TestFactory.Instance.GetHttpClient(userType, plant).PutAsync($"{_route}/SavedFilters/{id}", content);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
        public static async Task DeleteSavedFilterAsync(
            UserType userType,
            string plant,
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
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_route}/SavedFilters/{id}")
            {
                Content = new StringContent(serializePayload, Encoding.UTF8, "application/json")
            };

            var response = await TestFactory.Instance.GetHttpClient(userType, plant).SendAsync(request);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);
        }
    }
}
