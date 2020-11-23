using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    public static class TagsControllerTestsHelper
    {
        private const string _route = "Tags";

        public static async Task<TagResultDto> GetAllTagsAsync(
            HttpClient client,
            string projectName,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var parameters = new ParameterCollection {{"projectName", projectName}};
            var url = $"{_route}{parameters}";
            var response = await client.GetAsync(url);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TagResultDto>(content);
        }
        
        public static async Task<TagDetailsDto> GetTagAsync(
            HttpClient client,
            int tagId,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var response = await client.GetAsync($"{_route}/{tagId}");

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TagDetailsDto>(content);
        }

        public static async Task<int> DuplicateAreaTagAsync(
            HttpClient client,
            int sourceTagId,
            AreaTagType areaTagType,
            string disciplineCode,
            string areaCode,
            string tagNoSuffix,
            string description,
            string remark,
            string storageArea,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                sourceTagId,
                areaTagType,
                disciplineCode,
                areaCode,
                tagNoSuffix,
                description,
                remark,
                storageArea
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_route}/DuplicateArea", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return -1;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(jsonString);
        }

        public static async Task<string> UpdateTagStepAndRequirementsAsync(
            HttpClient client,
            int tagId,
            string description,
            int stepId,
            string rowVersion,
            List<TagRequirementDto> newRequirements = null,
            List<UpdatedTagRequirementDto> updatedRequirements = null,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                description,
                stepId,
                rowVersion,
                newRequirements,
                updatedRequirements
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_route}/{tagId}/UpdateTagStepAndRequirements", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<List<TagAttachmentDto>> GetAllTagAttachmentsAsync(
            HttpClient client,
            int tagId,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var response = await client.GetAsync($"{_route}/{tagId}/Attachments");

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<TagAttachmentDto>>(content);
        }

        public static async Task DeleteTagAttachmentAsync(
            HttpClient client,
            int tagId,
            int attachmentId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                rowVersion
            };
            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_route}/{tagId}/Attachments/{attachmentId}")
            {
                Content = new StringContent(serializePayload, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);
        }

        public static async Task<List<ActionAttachmentDto>> GetAllActionAttachmentsAsync(
            HttpClient client,
            int tagId,
            int actionId,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var response = await client.GetAsync($"{_route}/{tagId}/Actions/{actionId}/Attachments");

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ActionAttachmentDto>>(content);
        }

        public static async Task DeleteActionAttachmentAsync(
            HttpClient client,
            int tagId,
            int actionId,
            int attachmentId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                rowVersion
            };
            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_route}/{tagId}/Actions/{actionId}/Attachments/{attachmentId}")
            {
                Content = new StringContent(serializePayload, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);
        }

        public static async Task<List<ActionDto>> GetAllActionsAsync(
            HttpClient client,
            int tagId,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var response = await client.GetAsync($"{_route}/{tagId}/Actions");

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ActionDto>>(content);
        }

        public static async Task<string> UpdateActionAsync(
            HttpClient client,
            int tagId,
            int actionId,
            string title,
            string description,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                title,
                description,
                rowVersion
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_route}/{tagId}/Actions/{actionId}", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<ActionDetailsDto> GetActionAsync(
            HttpClient client,
            int tagId,
            int actionId,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var response = await client.GetAsync($"{_route}/{tagId}/Actions/{actionId}");

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ActionDetailsDto>(content);
        }

        public static async Task<string> CloseActionAsync(
            HttpClient client,
            int tagId,
            int actionId,
            string rowVersion,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                rowVersion
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_route}/{tagId}/Actions/{actionId}/Close", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task UploadActionAttachmentAsync(
            HttpClient client,
            int tagId,
            int actionId,
            TestFile file,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var httpContent = file.CreateHttpContent();
            var response = await client.PostAsync($"{_route}/{tagId}/Actions/{actionId}/Attachments", httpContent);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);
        }

        public static async Task<int> CreateActionAsync(
            HttpClient client,
            int tagId,
            string title,
            string description,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                title,
                description
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_route}/{tagId}/Actions", content);
            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return -1;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(jsonString);
        }

        public static async Task<List<RequirementDetailsDto>> GetTagRequirementsAsync(
            HttpClient client,
            int tagId,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var response = await client.GetAsync($"{_route}/{tagId}/Requirements");

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);

            if (expectedStatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<RequirementDetailsDto>>(content);
        }

        public static async Task UploadFieldValueAttachmentAsync(
            HttpClient client,
            int tagId,
            int requirementId,
            int fieldId,
            TestFile file,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var httpContent = file.CreateHttpContent();
            var response = await client.PostAsync($"{_route}/{tagId}/Requirements/{requirementId}/Attachment/{fieldId}", httpContent);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);
        }

        public static async Task RecordCbValueAsync(
            HttpClient client,
            int tagId,
            int requirementId,
            int fieldId,
            string comment,
            bool isChecked,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var bodyPayload = new
            {
                comment,
                checkBoxValues = new []
                {
                    new {
                        fieldId,
                        isChecked
                    }
                }
            };

            var serializePayload = JsonConvert.SerializeObject(bodyPayload);
            var content = new StringContent(serializePayload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_route}/{tagId}/Requirements/{requirementId}/RecordValues", content);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);
        }

        public static async Task<GetTagRequirementInfo> GetTagRequirementInfoAsync(HttpClient client, int tagId)
        {
            var requirementDetailDtos = await GetTagRequirementsAsync(client, tagId);
            var requirementDetailDto = requirementDetailDtos.First();
            Assert.IsNotNull(requirementDetailDto.NextDueTimeUtc, "Bad test setup: Preservation not started");
            Assert.AreEqual(1, requirementDetailDto.Fields.Count, "Bad test setup: Expect to find 1 requirement on tag under test");
            return new GetTagRequirementInfo(
                requirementDetailDto.Id,
                requirementDetailDto.NextDueTimeUtc.Value,
                requirementDetailDto.Fields);
        }

        public static async Task PreserveRequirementAsync(
            HttpClient client,
            int tagId,
            int requirementId,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            string expectedMessageOnBadRequest = null)
        {
            var response = await client.PostAsync($"{_route}/{tagId}/Requirements/{requirementId}/Preserve", null);

            await TestsHelper.AssertResponseAsync(response, expectedStatusCode, expectedMessageOnBadRequest);
        }
    }
}
