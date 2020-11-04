using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    [TestClass]
    public class TagsControllerTests : TestBase
    {
        private int initialTagsCount;
        private int initialTagId;

        [TestInitialize]
        public async Task TestInitialize()
        {
            var result = await TagsControllerTestsHelper.GetAllTagsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess);

            Assert.IsNotNull(result);

            initialTagsCount = result.MaxAvailable;
            Assert.IsTrue(initialTagsCount > 0, "Didn't find any tags at startup. Bad test setup");
            Assert.AreEqual(initialTagsCount, result.Tags.Count);
            initialTagId = result.Tags.First().Id;
        }

        [TestMethod]
        public async Task Get_Tag_AsPreserver_ShouldGetTag()
        {
            // Act
            var tag = await TagsControllerTestsHelper.GetTagAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                initialTagId);

            // Assert
            Assert.AreEqual(initialTagId, tag.Id);
            Assert.IsNotNull(tag.RowVersion);
        }

        [TestMethod]
        public async Task Get_Tags_ShouldReturnATagReadyToBeDuplicated()
        {
            // Act
            var result = await TagsControllerTestsHelper.GetAllTagsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess);

            // Assert
            var readyToBeDuplicatedTag = result.Tags.SingleOrDefault(t => t.IsReadyToBeDuplicated);
            Assert.IsNotNull(readyToBeDuplicatedTag);
        }
    }
}
