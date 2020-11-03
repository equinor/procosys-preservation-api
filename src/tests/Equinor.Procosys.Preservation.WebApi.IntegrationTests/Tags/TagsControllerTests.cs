using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    [TestClass]
    public class TagsControllerTests : TestBase
    {
        private int initialTagsCount;

        [TestInitialize]
        public async Task TestInitialize()
        {
            var result = await TagsControllerTestsHelper.GetAllTagsAsync(PreserverClient(TestFactory.PlantWithAccess), TestFactory.ProjectWithAccess);

            Assert.IsNotNull(result);

            initialTagsCount = result.MaxAvailable;
        }

        [TestMethod]
        public void Test()
        {
            // Act
            Assert.IsTrue(initialTagsCount > 0);
        }
    }
}
