using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    [TestClass]
    public class TagsControllerTests : TagsControllerTestsBase
    {
        [TestMethod]
        public async Task GetAllTags_AsPreserver_ShouldGetTags()
        {
            // Act
            var result = await TagsControllerTestsHelper.GetAllTagsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess);

            // Assert
            Assert.IsTrue(result.Tags.Count > 0);
        }

        [TestMethod]
        public async Task GetTag_AsPreserver_ShouldGetTag()
        {
            // Act
            var tag = await TagsControllerTestsHelper.GetTagAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                SiteAreaTagIdUnderTest);

            // Assert
            Assert.AreEqual(SiteAreaTagIdUnderTest, tag.Id);
            Assert.IsNotNull(tag.RowVersion);
            Assert.IsNotNull(tag.AreaCode);
            Assert.IsNotNull(tag.DisciplineCode);
        }

        [TestMethod]
        public async Task DuplicateAreaTag_AsPlanner_ShouldReturnATagReadyToBeDuplicated()
        {
            // Arrange
            var plannerClient = PlannerClient(TestFactory.PlantWithAccess);

            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                plannerClient,
                TestFactory.ProjectWithAccess);
            var initialTagsCount = tagsResult.Tags.Count;
            var readyToBeDuplicatedTag = tagsResult.Tags.SingleOrDefault(t => t.ReadyToBeDuplicated);
            Assert.IsNotNull(readyToBeDuplicatedTag, "Didn't find tag to duplicate. Bad test setup");

            // Act
            var id = await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                    plannerClient, 
                    readyToBeDuplicatedTag.Id, 
                    AreaTagType.SiteArea,
                    KnownDisciplineCode,
                    KnownAreaCode,
                    null,
                    "Desc",
                    null,
                    null);

            // Assert
            Assert.IsTrue(id > 0);
            tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                plannerClient,
                TestFactory.ProjectWithAccess);
            Assert.AreEqual(initialTagsCount+1, tagsResult.Tags.Count);
            Assert.IsNotNull(tagsResult.Tags.SingleOrDefault(t => t.Id == id));
        }

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPlanner_ShouldChangeDescriptionOnAreaTag()
        {
            // Arrange
            var plannerClient = PlannerClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = SiteAreaTagIdUnderTest;
            var tag = await TagsControllerTestsHelper.GetTagAsync(
                plannerClient, 
                tagIdUnderTest);
            var oldDescription = tag.Description;
            var newDescription = Guid.NewGuid().ToString();
            Assert.AreNotEqual(oldDescription, newDescription);
            var currentRowVersion = tag.RowVersion;

            // Act
            var newRowVersion = await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                plannerClient,
                tag.Id,
                newDescription,
                tag.Step.Id,
                tag.RowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            tag = await TagsControllerTestsHelper.GetTagAsync(
                plannerClient, 
                tagIdUnderTest);
            Assert.AreEqual(newDescription, tag.Description);
        }

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPlanner_ShouldKeepSameDescriptionOnStandardTag()
        {
            // Arrange
            var plannerClient = PlannerClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = StandardTagIdUnderTest;
            var tag = await TagsControllerTestsHelper.GetTagAsync(plannerClient, tagIdUnderTest);
            var oldDescription = tag.Description;
            var currentRowVersion = tag.RowVersion;

            // Act
            var newRowVersion = await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                plannerClient,
                tag.Id,
                oldDescription,
                tag.Step.Id,
                tag.RowVersion);

            // Assert
            Assert.AreEqual(currentRowVersion, newRowVersion);
            tag = await TagsControllerTestsHelper.GetTagAsync(plannerClient, tagIdUnderTest);
            Assert.AreEqual(oldDescription, tag.Description);
        }
    }
}
