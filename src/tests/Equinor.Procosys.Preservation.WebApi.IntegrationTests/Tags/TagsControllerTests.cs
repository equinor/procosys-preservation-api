﻿using System;
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
        public async Task ExportTagsToExcel_AsPreserver_ShouldGetAnExcelFile()
        {
            // Act
            var file = await TagsControllerTestsHelper.ExportTagsToExcelAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);

            // Assert
            Assert.IsNotNull(file);
            Assert.IsNotNull(file.Workbook);
            Assert.IsNotNull(file.Workbook.Worksheets);
            Assert.AreEqual(2, file.Workbook.Worksheets.Count);
            
            AssertFiltersSheet(file.Workbook.Worksheets.Worksheet("Filters"));

            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            var tag = tagsResult.Tags.Single(t => t.Id == tagIdUnderTest);
            var tagDetails = await TagsControllerTestsHelper.GetTagAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest);

            AssertTagsSheet(file.Workbook.Worksheets.Worksheet("Tags"), tag, tagDetails);
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

        private void AssertTagsSheet(IXLWorksheet worksheet, TagDto tag, TagDetailsDto tagDetailsDto)
        {
            var row = worksheet.Row(1);

            Assert.AreEqual(ExcelConverter.LastCol, row.CellsUsed().Count());
            Assert.AreEqual("Tag nr", row.Cell(ExcelConverter.TagNoCol).Value);
            Assert.AreEqual("Tag description", row.Cell(ExcelConverter.DescriptionCol).Value);
            Assert.AreEqual("Next preservation", row.Cell(ExcelConverter.NextCol).Value);
            Assert.AreEqual("Due (weeks)", row.Cell(ExcelConverter.DueCol).Value);
            Assert.AreEqual("Journey", row.Cell(ExcelConverter.JourneyCol).Value);
            Assert.AreEqual("Step", row.Cell(ExcelConverter.StepCol).Value);
            Assert.AreEqual("Mode", row.Cell(ExcelConverter.ModeCol).Value);
            Assert.AreEqual("Purchase order", row.Cell(ExcelConverter.PoCol).Value);
            Assert.AreEqual("Area", row.Cell(ExcelConverter.AreaCol).Value);
            Assert.AreEqual("Responsible", row.Cell(ExcelConverter.RespCol).Value);
            Assert.AreEqual("Discipline", row.Cell(ExcelConverter.DiscCol).Value);
            Assert.AreEqual("Status", row.Cell(ExcelConverter.PresStatusCol).Value);
            Assert.AreEqual("Requirements", row.Cell(ExcelConverter.ReqCol).Value);
            Assert.AreEqual("Remark", row.Cell(ExcelConverter.RemarkCol).Value);
            Assert.AreEqual("Storage area", row.Cell(ExcelConverter.StorageAreaCol).Value);
            Assert.AreEqual("Action status", row.Cell(ExcelConverter.ActionStatusCol).Value);
            Assert.AreEqual("Comm pkg", row.Cell(ExcelConverter.CommPkgCol).Value);
            Assert.AreEqual("MC pkg", row.Cell(ExcelConverter.McPkgCol).Value);
            Assert.AreEqual("Is voided", row.Cell(ExcelConverter.VoidedCol).Value);

            row = FindRowWithTag(worksheet, tag.TagNo);

            Assert.IsNotNull(row);
            Assert.AreEqual(tag.TagNo, row.Cell(ExcelConverter.TagNoCol).Value);
            Assert.AreEqual(tag.Description, row.Cell(ExcelConverter.DescriptionCol).Value);
            var firstRequirement = tag.Requirements.First();
            Assert.AreEqual(firstRequirement.NextDueAsYearAndWeek, row.Cell(ExcelConverter.NextCol).Value);
            Assert.AreEqual(firstRequirement.NextDueWeeks.ToString(), row.Cell(ExcelConverter.DueCol).Value.ToString());
            Assert.AreEqual(tagDetailsDto.Journey.Title, row.Cell(ExcelConverter.JourneyCol).Value);
            Assert.AreEqual(tagDetailsDto.Step.Title, row.Cell(ExcelConverter.StepCol).Value);
            Assert.AreEqual(tag.Mode, row.Cell(ExcelConverter.ModeCol).Value);
            Assert.AreEqual(PurchaseOrderHelper.CreateTitle(tag.PurchaseOrderNo, tag.CalloffNo), row.Cell(ExcelConverter.PoCol).Value);
            Assert.AreEqual(tag.AreaCode, row.Cell(ExcelConverter.AreaCol).Value);
            Assert.AreEqual(tag.ResponsibleCode, row.Cell(ExcelConverter.RespCol).Value);
            Assert.AreEqual(tag.DisciplineCode, row.Cell(ExcelConverter.DiscCol).Value);
            var status = Enum.Parse<PreservationStatus>(tag.Status);
            Assert.AreEqual(status.GetDisplayValue(), row.Cell(ExcelConverter.PresStatusCol).Value);
            var requirements = row.Cell(ExcelConverter.ReqCol).Value.ToString();
            // simple test for requirements since it is a comma-sep list of RequirementType titles which we don't have available here
            Assert.IsNotNull(requirements);
            var count = requirements.Count(comma => comma == ',') + 1;
            Assert.AreEqual(tag.Requirements.Count(), count);
            Assert.AreEqual(tagDetailsDto.Remark, row.Cell(ExcelConverter.RemarkCol).Value);
            Assert.AreEqual(tagDetailsDto.StorageArea, row.Cell(ExcelConverter.StorageAreaCol).Value);
            Assert.AreEqual(tag.ActionStatus.GetDisplayValue(), row.Cell(ExcelConverter.ActionStatusCol).Value);
            Assert.AreEqual(tag.CommPkgNo, row.Cell(ExcelConverter.CommPkgCol).Value);
            Assert.AreEqual(tag.McPkgNo, row.Cell(ExcelConverter.McPkgCol).Value);
            Assert.AreEqual(tag.IsVoided.ToString().ToUpper(), row.Cell(ExcelConverter.VoidedCol).Value.ToString()?.ToUpper());
        }

        private IXLRow FindRowWithTag(IXLWorksheet worksheet, string tagNo)
        {
            var rowsUsed = worksheet.RowsUsed().Count();
            for (var i = 2; i <= rowsUsed; i++)
            {
                var value = worksheet.Row(i).Cell(1).Value;
                if (value as string == tagNo)
                {
                    return worksheet.Row(i);
                }
            }

            return null;
        }

        private void AssertFiltersSheet(IXLWorksheet worksheet)
        {
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
