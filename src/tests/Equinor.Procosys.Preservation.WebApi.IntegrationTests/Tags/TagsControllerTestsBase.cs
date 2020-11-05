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
        protected int InitialTagId;
        private int InitialTagsCount;

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
            InitialTagId = result.Tags.First().Id;

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
