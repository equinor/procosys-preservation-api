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
            var tagResultDto = await TagsControllerTestsHelper.GetAllTagsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess);

            // Assert
            Assert.IsTrue(tagResultDto.Tags.Count > 0);

            var siteAreaTag = tagResultDto.Tags.Single(t => t.Id == SiteAreaTagIdUnderTest);
            Assert.IsNotNull(siteAreaTag.TagNo);
            Assert.IsNotNull(siteAreaTag.RowVersion);
        }

        [TestMethod]
        public async Task GetTag_AsPreserver_ShouldGetTag()
        {
            // Act
            var siteAreaTag = await TagsControllerTestsHelper.GetTagAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                SiteAreaTagIdUnderTest);

            // Assert
            Assert.AreEqual(SiteAreaTagIdUnderTest, siteAreaTag.Id);
            Assert.IsNotNull(siteAreaTag.TagNo);
            Assert.IsNotNull(siteAreaTag.RowVersion);
            Assert.IsNotNull(siteAreaTag.AreaCode);
            Assert.IsNotNull(siteAreaTag.DisciplineCode);
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

        [TestMethod]
        public async Task GetAllTagAttachments_AsPreserver_ShouldGetAttachments()
        {
            // Act
            var attachmentDtos = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest);

            // Assert
            Assert.IsNotNull(attachmentDtos);
            Assert.IsTrue(attachmentDtos.Count > 0);

            var standardTagAttachment = attachmentDtos.Single(t => t.Id == StandardTagAttachmentIdUnderTest);
            Assert.IsNotNull(standardTagAttachment.FileName);
            Assert.IsNotNull(standardTagAttachment.RowVersion);
        }

        [TestMethod]
        public async Task DeleteTagAttachment_AsPreserver_ShouldDeleteTagAttachment()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var attachmentDtos = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                preserverClient,
                StandardTagIdUnderTest);
            var standardTagAttachment = attachmentDtos.Single(t => t.Id == StandardTagAttachmentIdUnderTest);

            // Act
            await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                preserverClient,
                StandardTagIdUnderTest,
                StandardTagAttachmentIdUnderTest,
                standardTagAttachment.RowVersion);

            // Assert
            attachmentDtos = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                preserverClient,
                StandardTagIdUnderTest);
            Assert.IsNull(attachmentDtos.SingleOrDefault(m => m.Id == StandardTagAttachmentIdUnderTest));
        }

        [TestMethod]
        public async Task GetAllActionAttachments_AsPreserver_ShouldGetAttachments()
        {
            // Act
            var attachmentDtos = await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest);

            // Assert
            Assert.IsNotNull(attachmentDtos);
            Assert.IsTrue(attachmentDtos.Count > 0);

            var standardTagActionAttachment = attachmentDtos.Single(t => t.Id == StandardTagActionAttachmentIdUnderTest);
            Assert.IsNotNull(standardTagActionAttachment.FileName);
            Assert.IsNotNull(standardTagActionAttachment.RowVersion);
        }

        [TestMethod]
        public async Task DeleteActionAttachment_AsPreserver_ShouldDeleteActionAttachment()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var attachmentDtos = await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                preserverClient,
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest);
            var standardTagActionAttachment = attachmentDtos.Single(t => t.Id == StandardTagActionAttachmentIdUnderTest);

            // Act
            await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                preserverClient,
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest,
                StandardTagActionAttachmentIdUnderTest,
                standardTagActionAttachment.RowVersion);

            // Assert
            attachmentDtos = await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                preserverClient,
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest);
            Assert.IsNull(attachmentDtos.SingleOrDefault(m => m.Id == StandardTagActionAttachmentIdUnderTest));
        }

        [TestMethod]
        public async Task GetAllActions_AsPreserver_ShouldGetActions()
        {
            // Act
            var actionDtos = await TagsControllerTestsHelper.GetAllActionsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest);

            // Assert
            Assert.IsNotNull(actionDtos);
            Assert.IsTrue(actionDtos.Count > 0);

            var action = actionDtos.Single(t => t.Id == StandardTagActionIdUnderTest);
            Assert.IsNotNull(action.Title);
            Assert.IsNotNull(action.RowVersion);
        }
        
        [TestMethod]
        public async Task GetAction_AsPreserver_ShouldGetActionDetails()
        {
            // Act
            var actionDetailsDto = await TagsControllerTestsHelper.GetActionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                SiteAreaTagIdUnderTest,
                SiteAreaTagActionIdUnderTest);

            // Assert
            Assert.AreEqual(SiteAreaTagActionIdUnderTest, actionDetailsDto.Id);
            Assert.IsNotNull(actionDetailsDto.Title);
            Assert.IsNotNull(actionDetailsDto.Description);
            Assert.IsNotNull(actionDetailsDto.RowVersion);
        }

        [TestMethod]
        public async Task UpdateAction_AsPreserver_ShouldUpdateAction()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = StandardTagIdUnderTest;
            var actionIdUnderTest = StandardTagActionIdUnderTest;

            var actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                preserverClient, 
                tagIdUnderTest,
                actionIdUnderTest);
            var currentRowVersion = actionDetails.RowVersion;
            var newTitle = Guid.NewGuid().ToString();
            var newDescription = Guid.NewGuid().ToString();

            // Act
            var newRowVersion = await TagsControllerTestsHelper.UpdateActionAsync(
                preserverClient,
                tagIdUnderTest,
                actionIdUnderTest,
                newTitle,
                newDescription,
                currentRowVersion);
            
            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                preserverClient, 
                tagIdUnderTest,
                actionIdUnderTest);
            Assert.AreEqual(newTitle, actionDetails.Title);
            Assert.AreEqual(newDescription, actionDetails.Description);
        }

        [TestMethod]
        public async Task CloseAction_AsPreserver_ShouldCloseAction()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = StandardTagIdUnderTest;
            var actionIdUnderTest = await TagsControllerTestsHelper.CreateActionAsync(
                preserverClient,
                tagIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            var actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                preserverClient, 
                tagIdUnderTest,
                actionIdUnderTest);
            var currentRowVersion = actionDetails.RowVersion;
            Assert.IsNull(actionDetails.ClosedAtUtc);

            // Act
            var newRowVersion = await TagsControllerTestsHelper.CloseActionAsync(
                preserverClient,
                tagIdUnderTest,
                actionIdUnderTest,
                currentRowVersion);
            
            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                preserverClient, 
                tagIdUnderTest,
                actionIdUnderTest);
            Assert.IsNotNull(actionDetails.ClosedAtUtc);
        }

        [TestMethod]
        public async Task UploadActionAttachment_AsPreserver_ShouldUploadActionAttachment()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = StandardTagIdUnderTest;
            var actionIdUnderTest = StandardTagActionIdUnderTest;

            var actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                preserverClient, 
                tagIdUnderTest,
                actionIdUnderTest);
            var attachmentCount = actionDetails.AttachmentCount;

            // Act
            await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                preserverClient,
                tagIdUnderTest,
                actionIdUnderTest,
                FileToBeUploaded);
            
            // Assert
            actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                preserverClient, 
                tagIdUnderTest,
                actionIdUnderTest);
            Assert.AreEqual(attachmentCount + 1, actionDetails.AttachmentCount);
        }

        [TestMethod]
        public async Task CreateAction_AsPreserver_ShouldCreateAction()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = StandardTagIdUnderTest;
            var title = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();

            // Act
            var id = await TagsControllerTestsHelper.CreateActionAsync(
                preserverClient,
                tagIdUnderTest,
                title,
                description);
            
            // Assert
            var actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                preserverClient, 
                tagIdUnderTest,
                id);
            Assert.AreEqual(title, actionDetails.Title);
            Assert.AreEqual(description, actionDetails.Description);
        }
    }
}
