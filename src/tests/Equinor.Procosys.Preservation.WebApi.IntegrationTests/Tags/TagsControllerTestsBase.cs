using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Area;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Equinor.Procosys.Preservation.WebApi.Excel;
using Equinor.Procosys.Preservation.WebApi.IntegrationTests.Journeys;
using Equinor.Procosys.Preservation.WebApi.IntegrationTests.RequirementTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    [TestClass]
    public class TagsControllerTestsBase : TestBase
    {
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

        protected JourneyDto JourneyWithTags;

        protected TestFile FileToBeUploaded = new TestFile("test file content", "file.txt");
        protected TimeZoneInfo CetTimeZoneInfo;

        [TestInitialize]
        public async Task TestInitialize()
        {
            CetTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(ExcelConverter.CentralEuropeanTime);

            var result = await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);

            Assert.IsNotNull(result);

            InitialTagsCount = result.MaxAvailable;
            Assert.IsTrue(InitialTagsCount > 0, "Bad test setup: Didn't find any tags at startup");
            Assert.AreEqual(InitialTagsCount, result.Tags.Count);

            var journeys = await JourneysControllerTestsHelper.GetJourneysAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            JourneyWithTags = journeys.Single(j => j.Title == KnownTestData.JourneyWithTags);

            TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted
                = TestFactory.Instance.KnownTestData.TagId_ForStandardTagReadyForBulkPreserve_NotStarted;
            TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started
                = TestFactory.Instance.KnownTestData.TagId_ForStandardTagWithAttachmentRequirement_Started;
            TagIdUnderTest_ForStandardTagWithInfoRequirement_Started
                = TestFactory.Instance.KnownTestData.TagId_ForStandardTagWithInfoRequirement_Started;
            TagIdUnderTest_ForStandardTagWithCbRequirement_Started
                = TestFactory.Instance.KnownTestData.TagId_ForStandardTagWithCbRequirement_Started;
            TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted
                = TestFactory.Instance.KnownTestData.TagId_ForSiteAreaTagReadyForBulkPreserve_NotStarted;

            TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started
                = TestFactory.Instance.KnownTestData.TagId_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            TagIdUnderTest_ForSiteAreaTagWithAttachmentsAndActionAttachments_NotStarted
                = TestFactory.Instance.KnownTestData.TagId_ForSiteAreaTagWithAttachmentsAndActionAttachments_NotStarted;

            TestFactory.Instance
                .DisciplineApiServiceMock
                .Setup(service => service.TryGetDisciplineAsync(TestFactory.PlantWithAccess, KnownDisciplineCode))
                .Returns(Task.FromResult(new ProcosysDiscipline
                {
                    Code = KnownDisciplineCode, Description = $"{KnownDisciplineCode} - Description"
                }));

            TestFactory.Instance
                .AreaApiServiceMock
                .Setup(service => service.TryGetAreaAsync(TestFactory.PlantWithAccess, KnownAreaCode))
                .Returns(Task.FromResult(new ProcosysArea
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
