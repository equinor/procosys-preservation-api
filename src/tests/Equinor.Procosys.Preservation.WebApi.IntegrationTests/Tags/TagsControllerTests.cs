using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Query.GetTagsQueries;
using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using Equinor.Procosys.Preservation.WebApi.Excel;
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
                UserType.Preserver, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);

            // Assert
            Assert.IsTrue(tagsResult.Tags.Count > 0);

            var siteAreaTag = tagsResult.Tags.Single(t => t.Id == TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted);
            Assert.IsNotNull(siteAreaTag.TagNo);
            Assert.IsNotNull(siteAreaTag.RowVersion);
        }

        [TestMethod]
        public async Task ExportTagsToExcel_AsPreserver_ShouldGetACorrectExcelFile()
        {
            // Act
            var file = await TagsControllerTestsHelper.ExportTagsToExcelAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);

            // Assert
            Assert.IsNotNull(file);
            Assert.IsNotNull(file.Workbook);
            Assert.IsNotNull(file.Workbook.Worksheets);
            Assert.AreEqual(3, file.Workbook.Worksheets.Count);
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file.ContentType);
            
            AssertFiltersSheet(file.Workbook.Worksheets.Worksheet("Filters"));

            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            var tag = tagsResult.Tags.Single(t => t.Id == tagIdUnderTest);
            var tagDetails = await TagsControllerTestsHelper.GetTagAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                tagIdUnderTest);
            var actions = await TagsControllerTestsHelper.GetAllActionsAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                tagIdUnderTest);
            var attachments = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                tagIdUnderTest);
            
            Assert.IsTrue(actions.Count > 0, "Expect to find actions. Bad test setup");
            Assert.IsTrue(attachments.Count > 0, "Expect to find attachments. Bad test setup");
            
            AssertTagsSheet(file.Workbook.Worksheets.Worksheet("Tags"), tag, tagDetails, actions, attachments);
            AssertActionsSheet(file.Workbook.Worksheets.Worksheet("Actions"), tag, actions);
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
        public async Task DuplicateAreaTag_AsPlanner_ShouldReturnATagReadyToBeDuplicated()
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
            await AssertNewTagCreated(UserType.Planner, TestFactory.PlantWithAccess, id, initialTagsCount);
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

            // Act
            var updatedIntervalWeeks = requirementToUpdate.IntervalWeeks + 1;
            await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
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
            var newRequirements =  await TagsControllerTestsHelper.GetTagRequirementsAsync(UserType.Planner, TestFactory.PlantWithAccess, tagIdUnderTest);
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
            
            // Act
            var currentRowVersion = tagToTransfer.RowVersion;
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
            await AssertNewTagCreated(UserType.Planner, TestFactory.PlantWithAccess, id, initialTagsCount);
        }

        private async Task AssertNewTagCreated(
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
        
        private void AssertActionsSheet(IXLWorksheet worksheet, TagDto tag, List<ActionDto> actions)
        {
            Assert.IsNotNull(worksheet);
            var row = worksheet.Row(1);

            Assert.AreEqual(ExcelConverter.ActionSheetColumns.Last, row.CellsUsed().Count());
            Assert.AreEqual("Tag nr", row.Cell(ExcelConverter.ActionSheetColumns.TagNo).Value);
            Assert.AreEqual("Title", row.Cell(ExcelConverter.ActionSheetColumns.Title).Value);
            Assert.AreEqual("Description", row.Cell(ExcelConverter.ActionSheetColumns.Description).Value);
            Assert.AreEqual("Overdue", row.Cell(ExcelConverter.ActionSheetColumns.OverDue).Value);
            Assert.AreEqual("Due date (UTC)", row.Cell(ExcelConverter.ActionSheetColumns.DueTime).Value);
            Assert.AreEqual("Closed at (UTC)", row.Cell(ExcelConverter.ActionSheetColumns.ClosedAt).Value);

            var rows = FindRowsWithTag(worksheet, tag.TagNo);
            Assert.AreEqual(actions.Count, rows.Count);

            foreach (var action in actions)
            {
                row = FindRowWithAction(rows, action.Title);
                Assert.IsNotNull(row);

                var actionDetailsDto = TagsControllerTestsHelper.GetActionAsync(UserType.Preserver, TestFactory.PlantWithAccess, tag.Id, action.Id).Result;

                Assert.AreEqual(tag.TagNo, row.Cell(ExcelConverter.ActionSheetColumns.TagNo).Value);
                Assert.AreEqual(action.Title, row.Cell(ExcelConverter.ActionSheetColumns.Title).Value);
                Assert.AreEqual(action.IsOverDue.ToString().ToUpper(), row.Cell(ExcelConverter.ActionSheetColumns.OverDue).Value.ToString()?.ToUpper());
                Assert.AreEqual(actionDetailsDto.Description, row.Cell(ExcelConverter.ActionSheetColumns.Description).Value);
                AssertDates(actionDetailsDto.DueTimeUtc, row.Cell(ExcelConverter.ActionSheetColumns.DueTime).Value);
                AssertDates(actionDetailsDto.ClosedAtUtc, row.Cell(ExcelConverter.ActionSheetColumns.ClosedAt).Value);
            }
        }

        private void AssertDates(DateTime? expectedTimeUtc, object timeUtc)
        {
            if (expectedTimeUtc.HasValue)
            {
                Assert.AreEqual(expectedTimeUtc, timeUtc);
            }
            else
            {
                Assert.AreEqual(string.Empty, timeUtc);
            }
        }

        private void AssertTagsSheet(
            IXLWorksheet worksheet,
            TagDto tag,
            TagDetailsDto tagDetailsDto,
            List<ActionDto> actions,
            List<TagAttachmentDto> attachments)
        {
            Assert.IsNotNull(worksheet);
            var row = worksheet.Row(1);

            Assert.AreEqual(ExcelConverter.TagSheetColumns.Last, row.CellsUsed().Count());
            Assert.AreEqual("Tag nr", row.Cell(ExcelConverter.TagSheetColumns.TagNo).Value);
            Assert.AreEqual("Tag description", row.Cell(ExcelConverter.TagSheetColumns.Description).Value);
            Assert.AreEqual("Next preservation", row.Cell(ExcelConverter.TagSheetColumns.Next).Value);
            Assert.AreEqual("Due (weeks)", row.Cell(ExcelConverter.TagSheetColumns.DueWeeks).Value);
            Assert.AreEqual("Journey", row.Cell(ExcelConverter.TagSheetColumns.Journey).Value);
            Assert.AreEqual("Step", row.Cell(ExcelConverter.TagSheetColumns.Step).Value);
            Assert.AreEqual("Mode", row.Cell(ExcelConverter.TagSheetColumns.Mode).Value);
            Assert.AreEqual("Purchase order", row.Cell(ExcelConverter.TagSheetColumns.Po).Value);
            Assert.AreEqual("Area", row.Cell(ExcelConverter.TagSheetColumns.Area).Value);
            Assert.AreEqual("Responsible", row.Cell(ExcelConverter.TagSheetColumns.Resp).Value);
            Assert.AreEqual("Discipline", row.Cell(ExcelConverter.TagSheetColumns.Disc).Value);
            Assert.AreEqual("Status", row.Cell(ExcelConverter.TagSheetColumns.PresStatus).Value);
            Assert.AreEqual("Requirements", row.Cell(ExcelConverter.TagSheetColumns.Req).Value);
            Assert.AreEqual("Remark", row.Cell(ExcelConverter.TagSheetColumns.Remark).Value);
            Assert.AreEqual("Storage area", row.Cell(ExcelConverter.TagSheetColumns.StorageArea).Value);
            Assert.AreEqual("Comm pkg", row.Cell(ExcelConverter.TagSheetColumns.CommPkg).Value);
            Assert.AreEqual("MC pkg", row.Cell(ExcelConverter.TagSheetColumns.McPkg).Value);
            Assert.AreEqual("Action status", row.Cell(ExcelConverter.TagSheetColumns.ActionStatus).Value);
            Assert.AreEqual("Actions", row.Cell(ExcelConverter.TagSheetColumns.Actions).Value);
            Assert.AreEqual("Open actions", row.Cell(ExcelConverter.TagSheetColumns.OpenActions).Value);
            Assert.AreEqual("Overdue actions", row.Cell(ExcelConverter.TagSheetColumns.OverdueActions).Value);
            Assert.AreEqual("Attachments", row.Cell(ExcelConverter.TagSheetColumns.Attachments).Value);
            Assert.AreEqual("Is voided", row.Cell(ExcelConverter.TagSheetColumns.Voided).Value);

            row = FindRowWithTag(worksheet, tag.TagNo);

            Assert.AreEqual(tag.TagNo, row.Cell(ExcelConverter.TagSheetColumns.TagNo).Value);
            Assert.AreEqual(tag.Description, row.Cell(ExcelConverter.TagSheetColumns.Description).Value);
            var firstRequirement = tag.Requirements.First();
            Assert.AreEqual(firstRequirement.NextDueAsYearAndWeek, row.Cell(ExcelConverter.TagSheetColumns.Next).Value);
            Assert.IsTrue(firstRequirement.NextDueWeeks.HasValue);
            Assert.AreEqual((double)firstRequirement.NextDueWeeks, row.Cell(ExcelConverter.TagSheetColumns.DueWeeks).Value);
            Assert.AreEqual(tagDetailsDto.Journey.Title, row.Cell(ExcelConverter.TagSheetColumns.Journey).Value);
            Assert.AreEqual(tagDetailsDto.Step.Title, row.Cell(ExcelConverter.TagSheetColumns.Step).Value);
            Assert.AreEqual(tag.Mode, row.Cell(ExcelConverter.TagSheetColumns.Mode).Value);
            Assert.AreEqual(PurchaseOrderHelper.CreateTitle(tag.PurchaseOrderNo, tag.CalloffNo), row.Cell(ExcelConverter.TagSheetColumns.Po).Value);
            Assert.AreEqual(tag.AreaCode, row.Cell(ExcelConverter.TagSheetColumns.Area).Value);
            Assert.AreEqual(tag.ResponsibleCode, row.Cell(ExcelConverter.TagSheetColumns.Resp).Value);
            Assert.AreEqual(tag.DisciplineCode, row.Cell(ExcelConverter.TagSheetColumns.Disc).Value);
            var status = Enum.Parse<PreservationStatus>(tag.Status);
            Assert.AreEqual(status.GetDisplayValue(), row.Cell(ExcelConverter.TagSheetColumns.PresStatus).Value);
            var requirements = row.Cell(ExcelConverter.TagSheetColumns.Req).Value.ToString();
            // simple test for requirements since it is a comma-sep list of RequirementType titles which we don't have available here
            Assert.IsNotNull(requirements);
            var count = requirements.Count(comma => comma == ',') + 1;
            Assert.AreEqual(tag.Requirements.Count(), count);
            Assert.AreEqual(tagDetailsDto.Remark, row.Cell(ExcelConverter.TagSheetColumns.Remark).Value);
            Assert.AreEqual(tagDetailsDto.StorageArea, row.Cell(ExcelConverter.TagSheetColumns.StorageArea).Value);
            Assert.AreEqual(tag.ActionStatus.GetDisplayValue(), row.Cell(ExcelConverter.TagSheetColumns.ActionStatus).Value);
            Assert.AreEqual(tag.CommPkgNo, row.Cell(ExcelConverter.TagSheetColumns.CommPkg).Value);
            Assert.AreEqual(tag.McPkgNo, row.Cell(ExcelConverter.TagSheetColumns.McPkg).Value);
            Assert.AreEqual(tag.ActionStatus.GetDisplayValue(), row.Cell(ExcelConverter.TagSheetColumns.ActionStatus).Value);
            Assert.AreEqual((double)actions.Count, row.Cell(ExcelConverter.TagSheetColumns.Actions).Value);
            var openActions = actions.Count(a => !a.IsClosed);
            Assert.AreEqual((double)openActions, row.Cell(ExcelConverter.TagSheetColumns.OpenActions).Value);
            var overDueActions = actions.Count(a => !a.IsClosed && a.DueTimeUtc.HasValue && a.DueTimeUtc.Value < DateTime.UtcNow);
            Assert.AreEqual((double)overDueActions, row.Cell(ExcelConverter.TagSheetColumns.OverdueActions).Value);
            Assert.AreEqual((double)attachments.Count, row.Cell(ExcelConverter.TagSheetColumns.Attachments).Value);
            Assert.AreEqual(tag.IsVoided.ToString().ToUpper(), row.Cell(ExcelConverter.TagSheetColumns.Voided).Value.ToString()?.ToUpper());
        }

        private IXLRow FindRowWithTag(IXLWorksheet worksheet, string tagNo)
        {
            var rows = FindRowsWithTag(worksheet, tagNo);
            Assert.AreEqual(1, rows.Count, $"Expect to find 1 row with {tagNo}, but found {rows.Count}");
            return rows.Single();
        }

        private List<IXLRow> FindRowsWithTag(IXLWorksheet worksheet, string tagNo)
        {
            var rowsUsed = worksheet.RowsUsed().Count();
            var rows = new List<IXLRow>();
            for (var i = 2; i <= rowsUsed; i++)
            {
                var value = worksheet.Row(i).Cell(1).Value;
                if (value as string == tagNo)
                {
                    rows.Add(worksheet.Row(i));
                }
            }

            return rows;
        }
        
        private IXLRow FindRowWithAction(List<IXLRow> rows, string actionTitle)
        {
            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows.ElementAt(i);
                var value = row.Cell(2).Value;
                if (value as string == actionTitle)
                {
                    return row;
                }
            }

            return null;
        }

        private void AssertFiltersSheet(IXLWorksheet worksheet)
        {
            Assert.IsNotNull(worksheet);
            var row = worksheet.Row(1);
            Assert.AreEqual(1, row.CellsUsed().Count());
            Assert.AreEqual("Export of preserved tags", row.Cell(1).Value);
            
            row = worksheet.Row(3);
            Assert.AreEqual(2, row.CellsUsed().Count());
            Assert.AreEqual("Plant", row.Cell(1).Value);
            Assert.AreEqual(TestFactory.PlantWithAccess, row.Cell(2).Value);
        }
    }
}
