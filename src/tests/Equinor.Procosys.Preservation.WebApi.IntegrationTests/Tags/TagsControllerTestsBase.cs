using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Area;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    [TestClass]
    public class TagsControllerTestsBase : TestBase
    {
        protected readonly string KnownAreaCode = "A";
        protected readonly string KnownDisciplineCode = "D";
        protected int StandardTagIdUnderTest;
        protected int SiteAreaTagIdUnderTest;
        protected int StepIdUnderTest;
        protected int StandardTagActionIdUnderTest;
        protected int SiteAreaTagActionIdUnderTest;
        protected int InitialTagsCount;
        protected int StandardTagActionAttachmentIdUnderTest;
        protected int SiteAreaTagActionAttachmentIdUnderTest;

        [TestInitialize]
        public async Task TestInitialize()
        {
            var result = await TagsControllerTestsHelper.GetAllTagsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess);

            Assert.IsNotNull(result);

            InitialTagsCount = result.MaxAvailable;
            Assert.IsTrue(InitialTagsCount > 0, "Didn't find any tags at startup. Bad test setup");
            Assert.AreEqual(InitialTagsCount, result.Tags.Count);
            StandardTagIdUnderTest = TestFactory.KnownTestData.StandardTagIds.First();
            SiteAreaTagIdUnderTest = TestFactory.KnownTestData.SiteAreaTagIds.First();
            StepIdUnderTest = TestFactory.KnownTestData.StepIds.First();
            StandardTagActionIdUnderTest = TestFactory.KnownTestData.StandardTagActionIds.First();
            SiteAreaTagActionIdUnderTest = TestFactory.KnownTestData.SiteAreaTagActionIds.First();
            StandardTagActionAttachmentIdUnderTest = TestFactory.KnownTestData.StandardTagActionAttachmentIds.First();
            SiteAreaTagActionAttachmentIdUnderTest = TestFactory.KnownTestData.SiteAreaTagActionAttachmentIds.First();

            TestFactory
                .DisciplineApiServiceMock
                .Setup(service => service.TryGetDisciplineAsync(TestFactory.PlantWithAccess, KnownDisciplineCode))
                .Returns(Task.FromResult(new ProcosysDiscipline
                {
                    Code = KnownDisciplineCode, Description = $"{KnownDisciplineCode} - Description"
                }));

            TestFactory
                .AreaApiServiceMock
                .Setup(service => service.TryGetAreaAsync(TestFactory.PlantWithAccess, KnownAreaCode))
                .Returns(Task.FromResult(new ProcosysArea
                {
                    Code = KnownAreaCode, Description = $"{KnownAreaCode} - Description"
                }));
        }
    }
}
