using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags
{
    [TestClass]
    public class TagsControllerTests : TagsControllerTestsBase
    {
        [TestMethod]
        public async Task GetAllTags_AsPreserver_ShouldGetTags()
        {
            // Act
            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);

            // Assert
            Assert.IsTrue(tagsResult.Tags.Count > 0);

            var siteAreaTag = tagsResult.Tags.Single(t => t.Id == TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted);
            Assert.IsNotNull(siteAreaTag.TagNo);
            Assert.IsNotNull(siteAreaTag.RowVersion);
        }

        [TestMethod]
        public async Task ExportTagsToExcel_AsPreserver_ShouldGetAnExcelFile()
        {
            // Act
            var file = await TagsControllerTestsHelper.ExportTagsToExcelAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);

            // Assert
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file.ContentType);
        }

        [TestMethod]
        public async Task GetTag_AsPreserver_ShouldGetTag()
        {
            // Act
            var siteAreaTag = await TagsControllerTestsHelper.GetTagAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted);

            // Assert
            Assert.AreEqual(TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, siteAreaTag.Id);
            Assert.IsNotNull(siteAreaTag.TagNo);
            Assert.IsNotNull(siteAreaTag.RowVersion);
            Assert.IsNotNull(siteAreaTag.AreaCode);
            Assert.IsNotNull(siteAreaTag.DisciplineCode);
        }

        [TestMethod]
        public async Task DuplicateAreaTag_AsPlanner_ShouldDuplicateTag()
        {
            // Arrange
            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);
            var initialTagsCount = tagsResult.Tags.Count;
            var readyToBeDuplicatedTag = tagsResult.Tags.First(t => t.ReadyToBeDuplicated);
            Assert.IsNotNull(readyToBeDuplicatedTag, "Bad test setup: Didn't find tag to duplicate.");

            // Act
            var id = await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                    UserType.Planner, TestFactory.PlantWithAccess, 
                    readyToBeDuplicatedTag.Id, 
                    AreaTagType.SiteArea,
                    KnownDisciplineCode,
                    KnownAreaCode,
                    null,
                    "Desc",
                    null,
                    null);

            // Assert
            await AssertNewTagCreatedAsync(UserType.Planner, TestFactory.PlantWithAccess, id, initialTagsCount);
            await AssertInHistoryAsLatestEventAsync(id, UserType.Planner, EventType.TagCreated);
        }

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPlanner_ShouldChangeDescriptionOnAreaTag()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted;
            var tag = await TagsControllerTestsHelper.GetTagAsync(
                UserType.Planner, TestFactory.PlantWithAccess, 
                tagIdUnderTest);
            var oldDescription = tag.Description;
            var newDescription = Guid.NewGuid().ToString();
            Assert.AreNotEqual(oldDescription, newDescription);
            var currentRowVersion = tag.RowVersion;

            // Act
            var newRowVersion = await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                tag.Id,
                newDescription,
                tag.Step.Id,
                tag.RowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            tag = await TagsControllerTestsHelper.GetTagAsync(
                UserType.Planner, TestFactory.PlantWithAccess, 
                tagIdUnderTest);
            Assert.AreEqual(newDescription, tag.Description);
        }

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPlanner_ShouldKeepSameDescriptionOnStandardTag()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted;
            var tag = await TagsControllerTestsHelper.GetTagAsync(UserType.Planner, TestFactory.PlantWithAccess, tagIdUnderTest);
            var oldDescription = tag.Description;
            var currentRowVersion = tag.RowVersion;

            // Act
            var newRowVersion = await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                tag.Id,
                oldDescription,
                tag.Step.Id,
                tag.RowVersion);

            // Assert
            Assert.AreEqual(currentRowVersion, newRowVersion);
            tag = await TagsControllerTestsHelper.GetTagAsync(UserType.Planner, TestFactory.PlantWithAccess, tagIdUnderTest);
            Assert.AreEqual(oldDescription, tag.Description);
        }

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPlanner_ShouldUpdateAndAddRequirements()
        {
            // Arrange
            var newReqDefId = await CreateRequirementDefinitionAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);

            var tagIdUnderTest = TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted;
            var tag = await TagsControllerTestsHelper.GetTagAsync(UserType.Planner, TestFactory.PlantWithAccess,
                tagIdUnderTest);
            var oldDescription = tag.Description;
            var oldRequirements = await TagsControllerTestsHelper.GetTagRequirementsAsync(UserType.Planner,
                TestFactory.PlantWithAccess, tagIdUnderTest);
            var requirementToUpdate = oldRequirements.First();
            var updatedIntervalWeeks = requirementToUpdate.IntervalWeeks + 1;

            // Act
            await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                tagIdUnderTest,
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
            var newRequirements = await TagsControllerTestsHelper.GetTagRequirementsAsync(UserType.Planner, TestFactory.PlantWithAccess, tagIdUnderTest);
            Assert.AreEqual(oldRequirements.Count+1, newRequirements.Count);
            var newRequirement = newRequirements.SingleOrDefault(r => r.RequirementDefinition.Id == newReqDefId);
            Assert.IsNotNull(newRequirement);
            var updatedRequirement = newRequirements.SingleOrDefault(r => r.RequirementDefinition.Id == requirementToUpdate.Id);
            Assert.IsNotNull(updatedRequirement);
            Assert.AreEqual(updatedIntervalWeeks, updatedRequirement.IntervalWeeks);
       
            await AssertInHistoryAsLatestEventAsync(tagIdUnderTest, UserType.Planner, EventType.RequirementAdded);
            await AssertInHistoryAsExistingEventAsync(tagIdUnderTest, UserType.Planner, EventType.IntervalChanged);
        }

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPlanner_ShouldDeleteRequirement()
        {
            // Arrange
            var newReqDefId = await CreateRequirementDefinitionAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);

            var tagIdUnderTest = TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted;
            var tag = await TagsControllerTestsHelper.GetTagAsync(UserType.Planner, TestFactory.PlantWithAccess,
                tagIdUnderTest);
            await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                tag.Id,
                tag.Description,
                tag.Step.Id,
                tag.RowVersion,
                new List<TagRequirementDto>
                {
                    new TagRequirementDto
                    {
                        IntervalWeeks = 4,
                        RequirementDefinitionId = newReqDefId
                    }
                });
            var oldRequirements = await TagsControllerTestsHelper.GetTagRequirementsAsync(UserType.Planner,
                TestFactory.PlantWithAccess, tagIdUnderTest);
            Assert.IsTrue(oldRequirements.Count > 1);
            var requirementToDelete = oldRequirements.First();

            // Act
            await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                tag.Id,
                tag.Description,
                tag.Step.Id,
                tag.RowVersion,
                updatedRequirements: new List<UpdatedTagRequirementDto>
                {
                    new UpdatedTagRequirementDto
                    {
                        IntervalWeeks = requirementToDelete.IntervalWeeks,
                        IsVoided = true,
                        RequirementId = requirementToDelete.Id,
                        RowVersion = requirementToDelete.RowVersion
                    }
                },
                deletedRequirements: new List<DeletedTagRequirementDto>
                {
                    new DeletedTagRequirementDto
                    {
                        RequirementId = requirementToDelete.Id,
                        RowVersion = requirementToDelete.RowVersion
                    }
                });

            // Assert
            var newRequirements = await TagsControllerTestsHelper.GetTagRequirementsAsync(UserType.Planner, TestFactory.PlantWithAccess, tagIdUnderTest);
            Assert.AreEqual(oldRequirements.Count-1, newRequirements.Count);
            await AssertInHistoryAsLatestEventAsync(tagIdUnderTest, UserType.Planner, EventType.RequirementDeleted);
            await AssertInHistoryAsExistingEventAsync(tagIdUnderTest, UserType.Planner, EventType.RequirementVoided);
        }

        [TestMethod]
        public async Task GetAllTagAttachments_AsPreserver_ShouldGetAttachments()
        {
            // Act
            var attachmentDtos = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started);

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
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            var attachmentDtos = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest);
            var tagAttachment = attachmentDtos.First();

            // Act
            await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                tagAttachment.Id,
                tagAttachment.RowVersion);

            // Assert
            attachmentDtos = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest);
            Assert.IsNull(attachmentDtos.SingleOrDefault(m => m.Id == tagAttachment.Id));
        }

        [TestMethod]
        public async Task GetAllActionAttachments_AsPreserver_ShouldGetAttachments()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            var actionsDtos = await TagsControllerTestsHelper.GetAllActionsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest);
            var action = actionsDtos.First();

            // Act
            var attachmentDtos = await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
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
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            var actionsDtos = await TagsControllerTestsHelper.GetAllActionsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest);
            var action = actionsDtos.First();
            var actionAttachmentDtos = await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                action.Id);
            var actionAttachment = actionAttachmentDtos.First();

            // Act
            await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                action.Id,
                actionAttachment.Id,
                actionAttachment.RowVersion);

            // Assert
            actionAttachmentDtos = await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                action.Id);
            Assert.IsNull(actionAttachmentDtos.SingleOrDefault(m => m.Id == actionAttachment.Id));
        }

        [TestMethod]
        public async Task GetAllActions_AsPreserver_ShouldGetActions()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            var actionIdUnderTest = await TagsControllerTestsHelper.CreateActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            // Act
            var actionDtos = await TagsControllerTestsHelper.GetAllActionsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
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
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            var actionIdUnderTest = await TagsControllerTestsHelper.CreateActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            // Act
            var actionDetailsDto = await TagsControllerTestsHelper.GetActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                actionIdUnderTest);

            // Assert
            Assert.AreEqual(actionIdUnderTest, actionDetailsDto.Id);
            Assert.IsNotNull(actionDetailsDto.CreatedBy);
            Assert.IsNotNull(actionDetailsDto.Title);
            Assert.IsNotNull(actionDetailsDto.Description);
            Assert.IsNotNull(actionDetailsDto.RowVersion);
        }

        [TestMethod]
        public async Task UpdateAction_AsPreserver_ShouldUpdateAction()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            var actionIdUnderTest = await TagsControllerTestsHelper.CreateActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            var actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                tagIdUnderTest,
                actionIdUnderTest);
            var currentRowVersion = actionDetails.RowVersion;
            var newTitle = Guid.NewGuid().ToString();
            var newDescription = Guid.NewGuid().ToString();

            // Act
            var newRowVersion = await TagsControllerTestsHelper.UpdateActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                actionIdUnderTest,
                newTitle,
                newDescription,
                currentRowVersion);
            
            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                tagIdUnderTest,
                actionIdUnderTest);
            Assert.AreEqual(newTitle, actionDetails.Title);
            Assert.AreEqual(newDescription, actionDetails.Description);
        }

        [TestMethod]
        public async Task CloseAction_AsPreserver_ShouldCloseAction()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            var actionIdUnderTest = await TagsControllerTestsHelper.CreateActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            var actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                tagIdUnderTest,
                actionIdUnderTest);
            var currentRowVersion = actionDetails.RowVersion;
            Assert.IsNull(actionDetails.ClosedAtUtc);

            // Act
            var newRowVersion = await TagsControllerTestsHelper.CloseActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                actionIdUnderTest,
                currentRowVersion);
            
            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                tagIdUnderTest,
                actionIdUnderTest);
            Assert.IsNotNull(actionDetails.ClosedAtUtc);
            Assert.IsNotNull(actionDetails.ClosedBy);

            await AssertInHistoryAsLatestEventAsync(tagIdUnderTest, UserType.Preserver, EventType.ActionClosed);
        }

        [TestMethod]
        public async Task UploadActionAttachment_AsPreserver_ShouldUploadActionAttachment()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            var actionIdUnderTest = await TagsControllerTestsHelper.CreateActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

            var actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                tagIdUnderTest,
                actionIdUnderTest);
            var attachmentCount = actionDetails.AttachmentCount;

            // Act
            await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                actionIdUnderTest,
                FileToBeUploaded);
            
            // Assert
            actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                tagIdUnderTest,
                actionIdUnderTest);
            Assert.AreEqual(attachmentCount + 1, actionDetails.AttachmentCount);
        }

        [TestMethod]
        public async Task CreateAction_AsPreserver_ShouldCreateAction()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            var title = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();

            // Act
            var id = await TagsControllerTestsHelper.CreateActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                title,
                description);
            
            // Assert
            var actionDetails = await TagsControllerTestsHelper.GetActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                tagIdUnderTest,
                id);
            Assert.AreEqual(title, actionDetails.Title);
            Assert.AreEqual(description, actionDetails.Description);
            await AssertInHistoryAsLatestEventAsync(tagIdUnderTest, UserType.Preserver, EventType.ActionAdded);
        }

        [TestMethod]
        public async Task GetTagRequirements_AsPreserver_ShouldGetTagRequirements()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            
            // Act
            var requirementDetailDtos = await TagsControllerTestsHelper.GetTagRequirementsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
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
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;

            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest);
            Assert.IsNull(requirement.Fields.Single().CurrentValue, "Bad test setup: Attachment already uploaded");

            // Act
            await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                requirement.Id,
                requirement.Fields.First().Id,
                FileToBeUploaded);
            
            // Assert
            requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest);
            Assert.IsNotNull(requirement.Fields.Single().CurrentValue);
        }

        [TestMethod]
        public async Task RecordCbValueAsync_AsPreserver_ShouldRecordCbValueAsync()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithCbRequirement_Started;

            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest);
            Assert.IsNull(requirement.Fields.Single().CurrentValue, "Bad test setup: Value already recorded");
            var comment = Guid.NewGuid().ToString();

            // Act
            await TagsControllerTestsHelper.RecordCbValueAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                requirement.Id,
                requirement.Fields.First().Id,
                comment,
                true);
            
            // Assert
            requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest);
            Assert.IsNotNull(requirement.Fields.Single().CurrentValue);
        }

        [TestMethod]
        public async Task PreserveRequirement_AsPreserver_ShouldPreserveRequirement()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithInfoRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest);
            var oldNextDueTimeUtc = requirement.NextDueTimeUtc;

            // Act
            await TagsControllerTestsHelper.PreserveRequirementAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest, requirement.Id);

            // Assert
            requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest);
            Assert.AreNotEqual(oldNextDueTimeUtc, requirement.NextDueTimeUtc);
            await AssertInHistoryAsLatestEventAsync(tagIdUnderTest, UserType.Preserver, EventType.RequirementPreserved);
        }

        [TestMethod]
        public async Task Transfer_AsPlanner_ShouldTransferTags()
        {
            // Arrange 
            var newReqDefId = await CreateRequirementDefinitionAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            var stepId = JourneyWithTags.Steps.First().Id;

            var newTagId = await TagsControllerTestsHelper.CreateAreaTagAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
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
            await TagsControllerTestsHelper.StartPreservationAsync(UserType.Planner, TestFactory.PlantWithAccess, new List<int> {newTagId});

            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);
            var tagToTransfer = tagsResult.Tags.Single(t => t.Id == newTagId);
            Assert.IsTrue(tagToTransfer.ReadyToBeTransferred, "Bad test setup: Didn't find tag ready to be transferred");
            var currentRowVersion = tagToTransfer.RowVersion;
            
            // Act
            var idAndRowVersions = await TagsControllerTestsHelper.TransferAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
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
            await AssertInHistoryAsLatestEventAsync(tagToTransfer.Id, UserType.Planner, EventType.TransferredManually);
        }

        [TestMethod]
        public async Task CreateAreaTag_AsPlanner_ShouldCreateAreaTag()
        {
            // Arrange
            var newReqDefId = await CreateRequirementDefinitionAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            var stepId = JourneyWithTags.Steps.Last().Id;

            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);
            var initialTagsCount = tagsResult.Tags.Count;

            // Act
            var id = await TagsControllerTestsHelper.CreateAreaTagAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
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
            await AssertNewTagCreatedAsync(UserType.Planner, TestFactory.PlantWithAccess, id, initialTagsCount);
        }
        
        [TestMethod]
        public async Task GetHistory_AsPreserver_ShouldGetHistory()
        {
            // Arrange
            var newReqDefId = await CreateRequirementDefinitionAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            var stepId = JourneyWithTags.Steps.First().Id;

            var newTagId = await TagsControllerTestsHelper.CreateAreaTagAsync(
                UserType.Planner, 
                TestFactory.PlantWithAccess,
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
            await TagsControllerTestsHelper.StartPreservationAsync(UserType.Planner, TestFactory.PlantWithAccess, new List<int> {newTagId});

            // Act
            var historyDtos = await TagsControllerTestsHelper.GetHistoryAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                newTagId);

            // Assert
            Assert.IsNotNull(historyDtos);
            Assert.AreEqual(2, historyDtos.Count);

            var plannerProfile = TestFactory.Instance.GetTestProfile(UserType.Planner);
            // history records are sorted with newest first in list
            var historyDto = historyDtos.First();
            Assert.IsTrue(historyDto.Description.StartsWith(EventType.PreservationStarted.GetDescription()));
            AssertUser(plannerProfile, historyDto.CreatedBy);
            historyDto = historyDtos.Last();
            Assert.IsTrue(historyDto.Description.StartsWith(EventType.TagCreated.GetDescription()));
            AssertUser(plannerProfile, historyDto.CreatedBy);
        }

        [TestMethod]
        public async Task Reschedule_AsPlanner_ShouldRescheduleTags()
        {
            // Arrange 
            var newReqDefId = await CreateRequirementDefinitionAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            var stepId = JourneyWithTags.Steps.First().Id;

            var newTagId = await TagsControllerTestsHelper.CreateAreaTagAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
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
            await TagsControllerTestsHelper.StartPreservationAsync(UserType.Planner, TestFactory.PlantWithAccess, new List<int> {newTagId});

            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);
            var tagToReschedule = tagsResult.Tags.Single(t => t.Id == newTagId);
            
            // Act
            var currentRowVersion = tagToReschedule.RowVersion;
            var idAndRowVersions = await TagsControllerTestsHelper.RescheduleAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                new List<IdAndRowVersion>
                {
                    new IdAndRowVersion
                    {
                        Id = tagToReschedule.Id,
                        RowVersion = currentRowVersion
                    }
                },
                52,
                RescheduledDirection.Later,
                "Test");

            // Assert
            Assert.IsNotNull(idAndRowVersions);
            Assert.AreEqual(1, idAndRowVersions.Count);

            var requirementDetailDto = idAndRowVersions.Single();
            AssertRowVersionChange(currentRowVersion, requirementDetailDto.RowVersion);
            await AssertInHistoryAsLatestEventAsync(tagToReschedule.Id, UserType.Planner, EventType.Rescheduled);
        }

        private void AssertUser(TestProfile profile, PersonDto personDto)
        {
            Assert.IsNotNull(personDto);
            Assert.AreEqual(profile.FirstName, personDto.FirstName);
            Assert.AreEqual(profile.LastName, personDto.LastName);
        }

        private void AssertCreatedBy(UserType userType, HistoryDto historyDto)
        {
            var plannerProfile = TestFactory.Instance.GetTestProfile(userType);
            AssertUser(plannerProfile, historyDto.CreatedBy);
        }

        private async Task AssertNewTagCreatedAsync(
            UserType userType, 
            string plant,
            int id, 
            int initialTagsCount)
        {
            Assert.IsTrue(id > 0);
            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                userType,
                plant,
                TestFactory.ProjectWithAccess);
            Assert.AreEqual(initialTagsCount + 1, tagsResult.Tags.Count);
            Assert.IsNotNull(tagsResult.Tags.SingleOrDefault(t => t.Id == id));
        }

        private async Task AssertInHistoryAsExistingEventAsync(int tagIdUnderTest, UserType userType, EventType eventType)
        {
            var historyDtos = await TagsControllerTestsHelper.GetHistoryAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest);
            
            // history records are sorted with newest first in list
            var historyDto = historyDtos.First(h => h.Description.StartsWith(eventType.GetDescription()));
            AssertCreatedBy(userType, historyDto);
        }

        private async Task AssertInHistoryAsLatestEventAsync(int tagId, UserType userType, EventType eventType)
        {
            var historyDtos = await TagsControllerTestsHelper.GetHistoryAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagId);
            
            // history records are sorted with newest first in list
            var historyDto = historyDtos.First();
            Assert.IsTrue(historyDto.Description.StartsWith(eventType.GetDescription()));
            AssertCreatedBy(userType, historyDto);
        }
    }
}
