using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess);

            // Assert
            Assert.IsTrue(tagsResult.Tags.Count > 0);

            var siteAreaTag = tagsResult.Tags.Single(t => t.Id == TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted);
            Assert.IsNotNull(siteAreaTag.TagNo);
            Assert.IsNotNull(siteAreaTag.RowVersion);
        }

        [TestMethod]
        public async Task GetTag_AsPreserver_ShouldGetTag()
        {
            // Act
            var siteAreaTag = await TagsControllerTestsHelper.GetTagAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted);

            // Assert
            Assert.AreEqual(TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, siteAreaTag.Id);
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
            var readyToBeDuplicatedTag = tagsResult.Tags.First(t => t.ReadyToBeDuplicated);
            Assert.IsNotNull(readyToBeDuplicatedTag, "Bad test setup: Didn't find tag to duplicate.");

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
            await AssertNewTagCreated(plannerClient, id, initialTagsCount);
        }

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPlanner_ShouldChangeDescriptionOnAreaTag()
        {
            // Arrange
            var plannerClient = PlannerClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted;
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
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted;
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
        public async Task UpdateTagStepAndRequirements_AsPlanner_ShouldUpdateAndAddRequirements()
        {
            // Arrange
            var client = LibraryAdminClient(TestFactory.PlantWithAccess);
            var newReqDefId = await CreateRequirementDefinitionAsync(client);

            client = PlannerClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted;
            var tag = await TagsControllerTestsHelper.GetTagAsync(client, tagIdUnderTest);
            var oldDescription = tag.Description;
            var oldRequirements =  await TagsControllerTestsHelper.GetTagRequirementsAsync(client, tagIdUnderTest);
            var requirementToUpdate = oldRequirements.First();

            // Act
            var updatedIntervalWeeks = requirementToUpdate.IntervalWeeks + 1;
            await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                client,
                tag.Id,
                oldDescription,
                tag.Step.Id,
                tag.RowVersion,
                new List<TagRequirementDto>
                {
                    new TagRequirementDto
                    {
                        IntervalWeeks = 4,
                        RequirementDefinitionId = newReqDefId
                    }
                },
                new List<UpdatedTagRequirementDto>
                {
                    new UpdatedTagRequirementDto
                    {
                        IntervalWeeks = updatedIntervalWeeks,
                        IsVoided = false,
                        RequirementId = requirementToUpdate.Id,
                        RowVersion = requirementToUpdate.RowVersion
                    }
                });

            // Assert
            var newRequirements =  await TagsControllerTestsHelper.GetTagRequirementsAsync(client, tagIdUnderTest);
            Assert.AreEqual(oldRequirements.Count+1, newRequirements.Count);
            var newRequirement = newRequirements.SingleOrDefault(r => r.RequirementDefinition.Id == newReqDefId);
            Assert.IsNotNull(newRequirement);
            var updatedRequirement = newRequirements.SingleOrDefault(r => r.RequirementDefinition.Id == requirementToUpdate.Id);
            Assert.IsNotNull(updatedRequirement);
            Assert.AreEqual(updatedIntervalWeeks, updatedRequirement.IntervalWeeks);
        }

        [TestMethod]
        public async Task GetAllTagAttachments_AsPreserver_ShouldGetAttachments()
        {
            // Act
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var attachmentDtos = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                preserverClient,
                TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments);

            // Assert
            Assert.IsNotNull(attachmentDtos);
            Assert.IsTrue(attachmentDtos.Count > 0);

            var tagAttachment = attachmentDtos.First();
            Assert.IsNotNull(tagAttachment.FileName);
            Assert.IsNotNull(tagAttachment.RowVersion);
        }

        [TestMethod]
        public async Task DeleteTagAttachment_AsPreserver_ShouldDeleteTagAttachment()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments;
            var attachmentDtos = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                preserverClient,
                tagIdUnderTest);
            var tagAttachment = attachmentDtos.First();

            // Act
            await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                preserverClient,
                tagIdUnderTest,
                tagAttachment.Id,
                tagAttachment.RowVersion);

            // Assert
            attachmentDtos = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                preserverClient,
                tagIdUnderTest);
            Assert.IsNull(attachmentDtos.SingleOrDefault(m => m.Id == tagAttachment.Id));
        }

        [TestMethod]
        public async Task GetAllActionAttachments_AsPreserver_ShouldGetAttachments()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments;
            var actionsDtos = await TagsControllerTestsHelper.GetAllActionsAsync(
                preserverClient,
                tagIdUnderTest);
            var action = actionsDtos.First();

            // Act
            var attachmentDtos = await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                preserverClient,
                tagIdUnderTest,
                action.Id);

            // Assert
            Assert.IsNotNull(attachmentDtos);
            Assert.IsTrue(attachmentDtos.Count > 0);

            var standardTagActionAttachment = attachmentDtos.First();
            Assert.IsNotNull(standardTagActionAttachment.FileName);
            Assert.IsNotNull(standardTagActionAttachment.RowVersion);
        }

        [TestMethod]
        public async Task DeleteActionAttachment_AsPreserver_ShouldDeleteActionAttachment()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments;
            var actionsDtos = await TagsControllerTestsHelper.GetAllActionsAsync(
                preserverClient,
                tagIdUnderTest);
            var action = actionsDtos.First();
            var actionAttachmentDtos = await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                preserverClient,
                tagIdUnderTest,
                action.Id);
            var actionAttachment = actionAttachmentDtos.First();

            // Act
            await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                preserverClient,
                tagIdUnderTest,
                action.Id,
                actionAttachment.Id,
                actionAttachment.RowVersion);

            // Assert
            actionAttachmentDtos = await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                preserverClient,
                tagIdUnderTest,
                action.Id);
            Assert.IsNull(actionAttachmentDtos.SingleOrDefault(m => m.Id == actionAttachment.Id));
        }

        [TestMethod]
        public async Task GetAllActions_AsPreserver_ShouldGetActions()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments;
            var actionIdUnderTest = await TagsControllerTestsHelper.CreateActionAsync(
                preserverClient,
                tagIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            // Act
            var actionDtos = await TagsControllerTestsHelper.GetAllActionsAsync(
                preserverClient,
                tagIdUnderTest);

            // Assert
            Assert.IsNotNull(actionDtos);
            Assert.IsTrue(actionDtos.Count > 0);

            var action = actionDtos.Single(a => a.Id == actionIdUnderTest);
            Assert.IsNotNull(action.Title);
            Assert.IsNotNull(action.RowVersion);
        }
        
        [TestMethod]
        public async Task GetAction_AsPreserver_ShouldGetActionDetails()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments;
            var actionIdUnderTest = await TagsControllerTestsHelper.CreateActionAsync(
                preserverClient,
                tagIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            // Act
            var actionDetailsDto = await TagsControllerTestsHelper.GetActionAsync(
                preserverClient,
                tagIdUnderTest,
                actionIdUnderTest);

            // Assert
            Assert.AreEqual(actionIdUnderTest, actionDetailsDto.Id);
            Assert.IsNotNull(actionDetailsDto.Title);
            Assert.IsNotNull(actionDetailsDto.Description);
            Assert.IsNotNull(actionDetailsDto.RowVersion);
        }

        [TestMethod]
        public async Task UpdateAction_AsPreserver_ShouldUpdateAction()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments;
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
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments;
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
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments;
            var actionIdUnderTest = await TagsControllerTestsHelper.CreateActionAsync(
                preserverClient,
                tagIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

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
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments;
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

        [TestMethod]
        public async Task GetTagRequirements_AsPreserver_ShouldGetTagRequirements()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            
            // Act
            var requirementDetailDtos = await TagsControllerTestsHelper.GetTagRequirementsAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                tagIdUnderTest);
            
            // Assert
            Assert.IsNotNull(requirementDetailDtos);
            Assert.IsTrue(requirementDetailDtos.Count > 0);

            var requirementDetailDto = requirementDetailDtos.First();
            Assert.IsNotNull(requirementDetailDto.RowVersion);
        }

        [TestMethod]
        public async Task UploadFieldValueAttachment_AsPreserver_ShouldUploadFieldValueAttachment()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;

            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(preserverClient, tagIdUnderTest);
            Assert.IsNull(requirement.Fields.Single().CurrentValue, "Bad test setup: Attachment already uploaded");

            // Act
            await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                preserverClient,
                tagIdUnderTest,
                requirement.Id,
                requirement.Fields.First().Id,
                FileToBeUploaded);
            
            // Assert
            requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(preserverClient, tagIdUnderTest);
            Assert.IsNotNull(requirement.Fields.Single().CurrentValue);
        }

        [TestMethod]
        public async Task RecordCbValueAsync_AsPreserver_ShouldRecordCbValueAsync()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithCbRequirement_Started;

            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(preserverClient, tagIdUnderTest);
            Assert.IsNull(requirement.Fields.Single().CurrentValue, "Bad test setup: Value already recorded");
            var comment = Guid.NewGuid().ToString();

            // Act
            await TagsControllerTestsHelper.RecordCbValueAsync(
                preserverClient,
                tagIdUnderTest,
                requirement.Id,
                requirement.Fields.First().Id,
                comment,
                true);
            
            // Assert
            requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(preserverClient, tagIdUnderTest);
            Assert.IsNotNull(requirement.Fields.Single().CurrentValue);
        }

        [TestMethod]
        public async Task PreserveRequirement_AsPreserver_ShouldPreserveRequirement()
        {
            // Arrange
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithInfoRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(preserverClient, tagIdUnderTest);
            var oldNextDueTimeUtc = requirement.NextDueTimeUtc;

            // Act
            await TagsControllerTestsHelper.PreserveRequirementAsync(preserverClient, tagIdUnderTest, requirement.Id);

            // Assert
            requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(preserverClient, tagIdUnderTest);
            Assert.AreNotEqual(oldNextDueTimeUtc, requirement.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task Transfer_AsPlanner_ShouldTransferTags()
        {
            // Arrange 
            var client = LibraryAdminClient(TestFactory.PlantWithAccess);
            var newReqDefId = await CreateRequirementDefinitionAsync(client);
            var stepId = JourneyWithTags.Steps.First().Id;

            client = PlannerClient(TestFactory.PlantWithAccess);
            var newTagId = await TagsControllerTestsHelper.CreateAreaTagAsync(
                client,
                TestFactory.ProjectWithAccess,
                AreaTagType.PreArea,
                KnownDisciplineCode,
                KnownAreaCode,
                Guid.NewGuid().ToString(),
                new List<TagRequirementDto>
                {
                    new TagRequirementDto
                    {
                        IntervalWeeks = 4,
                        RequirementDefinitionId = newReqDefId
                    }
                },
                stepId,
                "Desc",
                null,
                null);
            await TagsControllerTestsHelper.StartPreservationAsync(client, new List<int> {newTagId});

            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                client,
                TestFactory.ProjectWithAccess);
            var tagToTransfer = tagsResult.Tags.Single(t => t.Id == newTagId);
            Assert.IsTrue(tagToTransfer.ReadyToBeTransferred, "Bad test setup: Didn't find tag ready to be transferred");
            
            // Act
            var currentRowVersion = tagToTransfer.RowVersion;
            var idAndRowVersions = await TagsControllerTestsHelper.TransferAsync(
                client,
                new List<IdAndRowVersion>
                {
                    new IdAndRowVersion
                    {
                        Id = tagToTransfer.Id,
                        RowVersion = currentRowVersion
                    }
                });

            // Assert
            Assert.IsNotNull(idAndRowVersions);
            Assert.AreEqual(1, idAndRowVersions.Count);

            var requirementDetailDto = idAndRowVersions.Single();
            AssertRowVersionChange(currentRowVersion, requirementDetailDto.RowVersion);
        }

        [TestMethod]
        public async Task CreateAreaTag_AsPlanner_ShouldCreateAreaTag()
        {
            // Arrange
            var client = LibraryAdminClient(TestFactory.PlantWithAccess);
            var newReqDefId = await CreateRequirementDefinitionAsync(client);
            var stepId = JourneyWithTags.Steps.Last().Id;

            client = PlannerClient(TestFactory.PlantWithAccess);
            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                client,
                TestFactory.ProjectWithAccess);
            var initialTagsCount = tagsResult.Tags.Count;

            // Act
            var id = await TagsControllerTestsHelper.CreateAreaTagAsync(
                client,
                TestFactory.ProjectWithAccess,
                AreaTagType.PreArea,
                KnownDisciplineCode,
                KnownAreaCode,
                Guid.NewGuid().ToString(),
                new List<TagRequirementDto>
                {
                    new TagRequirementDto
                    {
                        IntervalWeeks = 4,
                        RequirementDefinitionId = newReqDefId
                    }
                },
                stepId,
                "Desc",
                null,
                null);

            // Assert
            await AssertNewTagCreated(client, id, initialTagsCount);
        }

        private async Task AssertNewTagCreated(HttpClient client, int id, int initialTagsCount)
        {
            Assert.IsTrue(id > 0);
            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                client,
                TestFactory.ProjectWithAccess);
            Assert.AreEqual(initialTagsCount + 1, tagsResult.Tags.Count);
            Assert.IsNotNull(tagsResult.Tags.SingleOrDefault(t => t.Id == id));
        }
    }
}
