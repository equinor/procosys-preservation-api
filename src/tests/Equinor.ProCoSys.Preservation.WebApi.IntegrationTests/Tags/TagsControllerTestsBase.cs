using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Area;
using Equinor.ProCoSys.Preservation.MainApi.Discipline;
using Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags;
using Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Journeys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags
{
    public class TagsControllerTestsBase : TestBase
    {
        protected readonly string KnownPOCode = "PO";
        protected readonly string KnownAreaCode = "A";
        protected readonly string KnownDisciplineCode = "D";

        protected int TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted;
        protected int TagIdUnderTest_ForStandardTagWithInfoRequirement_Started;
        protected int TagIdUnderTest_ForStandardTagWithCbRequirement_Started;
        protected int TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
        protected int TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted;

        protected int TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
        protected int TagIdUnderTest_ForSiteAreaTagWithAttachmentsAndActionAttachments_NotStarted;

        protected JourneyDto TwoStepJourneyWithTags;

        protected TestFile FileToBeUploaded = new TestFile("test file content", "file.txt");

        [TestInitialize]
        public async Task TestInitialize()
        {
            var result = await TagsControllerTestsHelper.GetPageOfTagsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);

            Assert.IsNotNull(result);

            var initialTagsCount = result.MaxAvailable;
            Assert.IsTrue(initialTagsCount > 0, "Bad test setup: Didn't find any tags at startup");
            Assert.AreEqual(initialTagsCount, result.Tags.Count);

            var journeys = await JourneysControllerTestsHelper.GetJourneysAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            TwoStepJourneyWithTags = journeys.Single(j => j.Title == KnownTestData.TwoStepJourneyWithTags);

            TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted
                = TestFactory.Instance.SeededData[KnownPlantData.PlantA].TagId_ForStandardTagReadyForBulkPreserve_NotStarted;
            TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started
                = TestFactory.Instance.SeededData[KnownPlantData.PlantA].TagId_ForStandardTagWithAttachmentRequirement_Started;
            TagIdUnderTest_ForStandardTagWithInfoRequirement_Started
                = TestFactory.Instance.SeededData[KnownPlantData.PlantA].TagId_ForStandardTagWithInfoRequirement_Started;
            TagIdUnderTest_ForStandardTagWithCbRequirement_Started
                = TestFactory.Instance.SeededData[KnownPlantData.PlantA].TagId_ForStandardTagWithCbRequirement_Started;
            TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted
                = TestFactory.Instance.SeededData[KnownPlantData.PlantA].TagId_ForSiteAreaTagReadyForBulkPreserve_NotStarted;

            TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started
                = TestFactory.Instance.SeededData[KnownPlantData.PlantA].TagId_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            TagIdUnderTest_ForSiteAreaTagWithAttachmentsAndActionAttachments_NotStarted
                = TestFactory.Instance.SeededData[KnownPlantData.PlantA].TagId_ForSiteAreaTagWithAttachmentsAndActionAttachments_NotStarted;

            TestFactory.Instance
                .DisciplineApiServiceMock
                .Setup(service => service.TryGetDisciplineAsync(TestFactory.PlantWithAccess, KnownDisciplineCode, CancellationToken.None))
                .Returns(Task.FromResult(new PCSDiscipline
                {
                    Code = KnownDisciplineCode, Description = $"{KnownDisciplineCode} - Description"
                }));

            TestFactory.Instance
                .AreaApiServiceMock
                .Setup(service => service.TryGetAreaAsync(TestFactory.PlantWithAccess, KnownAreaCode, CancellationToken.None))
                .Returns(Task.FromResult(new PCSArea
                {
                    Code = KnownAreaCode, Description = $"{KnownAreaCode} - Description"
                }));
        }


        protected async Task<TagDetailsDto> CreateAndGetAreaTagAsync(
            AreaTagType areaTagType,
            int stepId,
            string purchaseOrderCalloffCode,
            bool startPreservation)
        {
            var tagIdUnderTest = await CreateAreaTagAsync(
                areaTagType,
                stepId,
                purchaseOrderCalloffCode,
                startPreservation);
            
            return await TagsControllerTestsHelper.GetTagAsync(UserType.Planner, TestFactory.PlantWithAccess, tagIdUnderTest);
        }

        protected async Task<int> CreateAreaTagAsync(
            AreaTagType areaTagType,
            int stepId,
            string purchaseOrderCalloffCode,
            bool startPreservation)
        {
            var newReqDefId = await CreateRequirementDefinitionAsync(TestFactory.PlantWithAccess);

            var newTagId = await TagsControllerTestsHelper.CreateAreaTagAsync(
                UserType.Planner,
                TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess,
                areaTagType,
                KnownDisciplineCode,
                KnownAreaCode,
                $"Title_{Guid.NewGuid()}",
                new List<TagRequirementDto>
                {
                    new TagRequirementDto
                    {
                        IntervalWeeks = 4,
                        RequirementDefinitionId = newReqDefId
                    }
                },
                stepId,
                $"Desc_{Guid.NewGuid()}",
                null,
                null,
                purchaseOrderCalloffCode);

            if (startPreservation)
            {
                await TagsControllerTestsHelper.StartPreservationAsync(UserType.Planner, TestFactory.PlantWithAccess, new List<int> { newTagId });
            }
            return newTagId;
        }

        public async Task<GetTagRequirementInfo> GetTagRequirementInfoAsync(UserType userType, string plant, int tagId)
        {
            var requirementDetailDtos = await TagsControllerTestsHelper.GetTagRequirementsAsync(userType, plant, tagId);
            var requirementDetailDto = requirementDetailDtos.First();
            Assert.IsNotNull(requirementDetailDto.NextDueTimeUtc, "Bad test setup: Preservation not started");
            Assert.AreEqual(1, requirementDetailDto.Fields.Count, "Bad test setup: Expect to find 1 requirement on tag under test");
            return new GetTagRequirementInfo(
                requirementDetailDto.Id,
                requirementDetailDto.NextDueTimeUtc.Value,
                requirementDetailDto.Fields);
        }
    }
}
