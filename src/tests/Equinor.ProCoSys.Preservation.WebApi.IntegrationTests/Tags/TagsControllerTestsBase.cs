using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Area;
using Equinor.ProCoSys.Preservation.MainApi.Discipline;
using Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Journeys;
using Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.RequirementTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags
{
    public class TagsControllerTestsBase : TestBase
    {
        protected readonly string KnownPOCode = "PO";
        protected readonly string KnownAreaCode = "A";
        protected readonly string KnownDisciplineCode = "D";

        protected int InitialTagsCount;
        
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

            InitialTagsCount = result.MaxAvailable;
            Assert.IsTrue(InitialTagsCount > 0, "Bad test setup: Didn't find any tags at startup");
            Assert.AreEqual(InitialTagsCount, result.Tags.Count);

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
                .Setup(service => service.TryGetDisciplineAsync(TestFactory.PlantWithAccess, KnownDisciplineCode))
                .Returns(Task.FromResult(new PCSDiscipline
                {
                    Code = KnownDisciplineCode, Description = $"{KnownDisciplineCode} - Description"
                }));

            TestFactory.Instance
                .AreaApiServiceMock
                .Setup(service => service.TryGetAreaAsync(TestFactory.PlantWithAccess, KnownAreaCode))
                .Returns(Task.FromResult(new PCSArea
                {
                    Code = KnownAreaCode, Description = $"{KnownAreaCode} - Description"
                }));
        }

        protected async Task<int> CreateRequirementDefinitionAsync(UserType userType, string plant)
        {
            var reqTypes = await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(userType, plant);
            var newReqDefId = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                userType, plant, reqTypes.First().Id, Guid.NewGuid().ToString());
            return newReqDefId;
        }
    }
}
