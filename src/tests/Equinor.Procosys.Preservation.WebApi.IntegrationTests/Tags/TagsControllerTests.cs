using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    [TestClass]
    public class TagsControllerTests : TagsControllerTestsBase
    {
        [TestMethod]
        public async Task Get_Tag_AsPreserver_ShouldGetTag()
        {
            // Act
            var tag = await TagsControllerTestsHelper.GetTagAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                InitialTagId);

            // Assert
            Assert.AreEqual(InitialTagId, tag.Id);
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
            var readyToBeDuplicatedTag = result.Tags.SingleOrDefault(t => t.ReadyToBeDuplicated);
            Assert.IsNotNull(readyToBeDuplicatedTag);
        }
    }
}
