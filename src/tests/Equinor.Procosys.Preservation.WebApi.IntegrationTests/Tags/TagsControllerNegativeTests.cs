using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    [TestClass]
    public class TagsControllerNegativeTests : TagsControllerTestsBase
    {
        private int _tagId1_WithAttachment;
        private int _tagAttachmentId1;
        private int _tagId2_WithAttachment;
        private int _tagAttachmentId2;

        private int _tagId1_WithAction;
        private int _tagActionId1;
        private int _tagActionAttachmentId1;
        private int _tagId2_WithAction;
        private int _tagActionId2;
        private int _tagActionAttachmentId2;

        [TestInitialize]
        public async Task Setup()
        {

            _tagId1_WithAttachment = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            var attachmentDtos = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                _tagId1_WithAttachment);
            _tagAttachmentId1 = attachmentDtos.First().Id;

            _tagId2_WithAttachment = TagIdUnderTest_ForSiteAreaTagWithAttachmentsAndActionAttachments_NotStarted;
            attachmentDtos = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                _tagId2_WithAttachment);
            _tagAttachmentId2 = attachmentDtos.First().Id;

            _tagId1_WithAction = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            var actionDtos = await TagsControllerTestsHelper.GetAllActionsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                _tagId1_WithAction);
            _tagActionId1 = actionDtos.First().Id;

            _tagId2_WithAction = TagIdUnderTest_ForSiteAreaTagWithAttachmentsAndActionAttachments_NotStarted;
            actionDtos = await TagsControllerTestsHelper.GetAllActionsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                _tagId2_WithAction);
            _tagActionId2 = actionDtos.First().Id;

            var actionAttachmentDtos = await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                _tagId1_WithAction,
                _tagActionId1);
            _tagActionAttachmentId1 = actionAttachmentDtos.First().Id;

            actionAttachmentDtos = await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                _tagId2_WithAction,
                _tagActionId2);
            _tagActionAttachmentId2 = actionAttachmentDtos.First().Id;

        }

        #region GetAllTags
        [TestMethod]
        public async Task GetAllTags_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                TestFactory.ProjectWithAccess,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllTags_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                TestFactory.ProjectWithAccess,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTags_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                TestFactory.ProjectWithAccess,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTags_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllTags_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess,
                HttpStatusCode.Forbidden);
        #endregion

        #region ExportTagsToExcel
        [TestMethod]
        public async Task ExportTagsToExcel_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.ExportTagsToExcelAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                TestFactory.ProjectWithAccess,
                expectedStatusCode:HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task ExportTagsToExcel_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.ExportTagsToExcelAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                TestFactory.ProjectWithAccess,
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"is not a valid plant");

        [TestMethod]
        public async Task ExportTagsToExcel_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.ExportTagsToExcelAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                TestFactory.ProjectWithAccess,
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"is not a valid plant");

        [TestMethod]
        public async Task ExportTagsToExcel_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.ExportTagsToExcelAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task ExportTagsToExcel_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.ExportTagsToExcelAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess,
                expectedStatusCode:HttpStatusCode.Forbidden);
        #endregion
        
        #region GetTag
        [TestMethod]
        public async Task GetTag_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetTagAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetTag_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetTagAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetTag_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetTagAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999, 
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetTag_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetTagAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTag_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetTagAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess, 
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTag_AsPlanner_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetTagAsync(
                UserType.Planner, TestFactory.PlantWithAccess, 
                9999, 
                HttpStatusCode.NotFound);

        [TestMethod]
        public async Task GetTag_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetTagAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                9999, 
                HttpStatusCode.NotFound);
        #endregion
        
        #region DuplicateAreaTag
        [TestMethod]
        public async Task DuplicateAreaTag_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                KnownAreaCode,
                null,
                "Desc",
                null,
                null,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DuplicateAreaTag_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                KnownAreaCode,
                null,
                "Desc",
                null,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DuplicateAreaTag_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                KnownAreaCode,
                null,
                "Desc",
                null,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DuplicateAreaTag_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999, 
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                KnownAreaCode,
                null,
                "Desc",
                null,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DuplicateAreaTag_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess, 
                9999, 
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                KnownAreaCode,
                null,
                "Desc",
                null,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DuplicateAreaTag_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                9999, 
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                KnownAreaCode,
                null,
                "Desc",
                null,
                null,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region UpdateTagStepAndRequirements
        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                null,
                1111,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                null,
                1111,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"is not a valid plant");

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                null,
                1111,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"is not a valid plant");

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999,
                null,
                5555,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                9999,
                null,
                5555,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                null,
                5555,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPlanner_ShouldReturnBadRequest_WhenChangeDescriptionOnStandardTag()
        {
            // Arrange
            var tag = await TagsControllerTestsHelper.GetTagAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                _tagId1_WithAttachment);
            var oldDescription = tag.Description;
            var newDescription = Guid.NewGuid().ToString();
            Assert.AreNotEqual(oldDescription, newDescription);

            // Act
            await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                tag.Id,
                newDescription,
                tag.Step.Id,
                tag.RowVersion,
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"Tag must be an area tag to update description!");
        }
        #endregion

        #region GetAllTagAttachments
        [TestMethod]
        public async Task GetAllTagAttachments_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                _tagId1_WithAttachment,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllTagAttachments_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                _tagId1_WithAttachment,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTagAttachments_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                _tagId1_WithAttachment,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTagAttachments_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                _tagId1_WithAttachment,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllTagAttachments_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                _tagId1_WithAttachment,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllTagAttachments_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                HttpStatusCode.NotFound);

        #endregion

        #region DeleteTagAttachment
        [TestMethod]
        public async Task DeleteTagAttachment_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                _tagId1_WithAttachment,
                _tagAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteTagAttachment_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                _tagId1_WithAttachment,
                _tagAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteTagAttachment_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                _tagId1_WithAttachment,
                _tagAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteTagAttachment_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999,
                5555,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteTagAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                9999,
                5555,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteTagAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                _tagAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag and/or attachment doesn't exist!");

        [TestMethod]
        public async Task DeleteTagAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownAttachmentId()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                _tagId1_WithAttachment,
                _tagAttachmentId2,  // known attachmentId, but under other Tag
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag and/or attachment doesn't exist!");
        #endregion
        
        #region GetAllActionAttachments
        [TestMethod]
        public async Task GetAllActionAttachments_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                _tagId1_WithAttachment,
                _tagActionId1,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllActionAttachments_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                _tagId1_WithAttachment,
                _tagActionId1,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActionAttachments_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                _tagId1_WithAttachment,
                _tagActionId1,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActionAttachments_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999,
                5555,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActionAttachments_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                9999,
                5555,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActionAttachments_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                _tagActionId2,
                HttpStatusCode.NotFound);

        [TestMethod]
        public async Task GetAllActionAttachments_AsPreserver_ShouldReturnNotFound_WhenUnknownActionId()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                _tagId1_WithAttachment,
                _tagActionId2, // known actionId, but under other tag
                HttpStatusCode.NotFound);

        #endregion

        #region DeleteActionAttachment
        [TestMethod]
        public async Task DeleteActionAttachment_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                _tagId1_WithAttachment,
                _tagActionId1,
                _tagActionAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteActionAttachment_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                _tagId1_WithAttachment,
                _tagActionId1,
                _tagActionAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteActionAttachment_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                _tagId1_WithAttachment,
                _tagActionId1,
                _tagActionAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteActionAttachment_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999,
                5555,
                4444,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteActionAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                9999,
                5555,
                4444,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                _tagActionId1,
                _tagActionAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag, action and/or attachment doesn't exist!");

        [TestMethod]
        public async Task DeleteActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownActionId()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                _tagId1_WithAttachment,
                _tagActionId2, // known actionId, but under other tag
                _tagActionAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag, action and/or attachment doesn't exist!");

        [TestMethod]
        public async Task DeleteActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownAttachmentId()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                _tagId1_WithAttachment,
                _tagActionId1,
                _tagActionAttachmentId2,  // known attachmentId, but under other action
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag, action and/or attachment doesn't exist!");

        #endregion
        
        #region GetAllActions
        [TestMethod]
        public async Task GetAllActions_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllActions_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActions_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActions_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActions_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActions_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                HttpStatusCode.NotFound);

        #endregion
        
        #region UpdateAction
        [TestMethod]
        public async Task UpdateAction_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                null,
                null,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateAction_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                null,
                null,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateAction_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                8888,
                null,
                null,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateAction_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999, 
                8888,
                null,
                null,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess, 
                9999, 
                8888,
                null,
                null,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999, 
                _tagActionId2,
                "TestTitle",
                "TestDescription",
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag and/or action doesn't exist!");

        [TestMethod]
        public async Task UpdateAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownActionId()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                _tagActionId1,   // known actionId, but under other Tag
                "TestTitle",
                "TestDescription",
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag and/or action doesn't exist!");
        #endregion
        
        #region GetAction
        [TestMethod]
        public async Task GetAction_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetActionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAction_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetActionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAction_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetActionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999, 
                8888,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAction_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetActionAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999,
                8888,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetActionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess, 
                9999, 
                8888,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAction_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                9999, 
                _tagActionId2,
                HttpStatusCode.NotFound);

        [TestMethod]
        public async Task GetAction_AsPreserver_ShouldReturnNotFound_WhenUnknownActionId()
            => await TagsControllerTestsHelper.GetActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                _tagActionId1,   // known actionId, but under other Tag
                HttpStatusCode.NotFound);
        #endregion
        
        #region CloseAction
        [TestMethod]
        public async Task CloseAction_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.CloseActionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CloseAction_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CloseActionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CloseAction_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CloseActionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                8888,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CloseAction_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CloseActionAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999, 
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CloseAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CloseActionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess, 
                9999, 
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task CloseAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.CloseActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999, 
                _tagActionId2,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag and/or action doesn't exist!");

        [TestMethod]
        public async Task CloseAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownActionId()
            => await TagsControllerTestsHelper.CloseActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                _tagActionId1,   // known actionId, but under other Tag
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag and/or action doesn't exist!");
        #endregion
        
        #region UploadActionAttachment
        [TestMethod]
        public async Task UploadActionAttachment_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                FileToBeUploaded,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UploadActionAttachment_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UploadActionAttachment_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                8888,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UploadActionAttachment_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999, 
                8888,
                FileToBeUploaded,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UploadActionAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess, 
                9999, 
                8888,
                FileToBeUploaded,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task UploadActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999, 
                _tagActionId2,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "Tag and/or action doesn't exist!");

        [TestMethod]
        public async Task UploadActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownActionId()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                _tagActionId1,   // known actionId, but under other Tag
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "Tag and/or action doesn't exist!");
        #endregion
        
        #region CreateAction
        [TestMethod]
        public async Task CreateAction_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.CreateActionAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                null,
                null,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateAction_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CreateActionAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                null,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateAction_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CreateActionAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                null,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateAction_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CreateActionAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999, 
                null,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CreateActionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess, 
                9999, 
                null,
                null,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task CreateAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.CreateActionAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999, 
                "TestTitle",
                "TestDescription",
                HttpStatusCode.BadRequest,
                "Tag doesn't exist!");
        #endregion
        
        #region GetTagRequirementsAsync
        [TestMethod]
        public async Task GetTagRequirementsAsync_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetTagRequirementsAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetTagRequirementsAsync_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetTagRequirementsAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetTagRequirementsAsync_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetTagRequirementsAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999, 
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetTagRequirementsAsync_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetTagRequirementsAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTagRequirements_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetTagRequirementsAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess, 
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTagRequirements_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetTagRequirementsAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                9999, 
                HttpStatusCode.NotFound);
        #endregion
        
        #region UploadFieldValueAttachment
        [TestMethod]
        public async Task UploadFieldValueAttachment_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                7777,
                FileToBeUploaded,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UploadFieldValueAttachment_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                7777,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UploadFieldValueAttachment_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                8888,
                7777,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UploadFieldValueAttachment_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999, 
                8888,
                7777,
                FileToBeUploaded,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UploadFieldValueAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess, 
                9999, 
                8888,
                7777,
                FileToBeUploaded,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task UploadFieldValueAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                requirement.Id,
                requirement.Fields.First().Id,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "Tag and/or requirement doesn't exist!");
        }

        [TestMethod]
        public async Task UploadFieldValueAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownRequirementId()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                8888,
                requirement.Fields.First().Id,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "Tag and/or requirement doesn't exist!");
        }

        [TestMethod]
        public async Task UploadFieldValueAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownFieldId()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                requirement.Id,
                7777,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "Field doesn't exist in requirement!");
        }

        #endregion
        
        #region PreserveRequirement
        [TestMethod]
        public async Task PreserveRequirement_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.PreserveRequirementAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task PreserveRequirement_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.PreserveRequirementAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task PreserveRequirement_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.PreserveRequirementAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                8888,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task PreserveRequirement_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.PreserveRequirementAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999,
                8888,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task PreserveRequirement_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.PreserveRequirementAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess, 
                9999,
                8888,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task PreserveRequirement_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.PreserveRequirementAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                requirement.Id,
                HttpStatusCode.BadRequest,
                "Tag and/or requirement doesn't exist!");
        }

        [TestMethod]
        public async Task PreserveRequirement_AsPreserver_ShouldReturnBadRequest_WhenUnknownRequirementId()
            => await TagsControllerTestsHelper.PreserveRequirementAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started,
                8888,
                HttpStatusCode.BadRequest,
                "Tag and/or requirement doesn't exist!");

        #endregion
        
        #region RecordCbValue
        [TestMethod]
        public async Task RecordCbValue_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.RecordCbValueAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                8888,
                7777,
                null,
                true,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task RecordCbValue_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.RecordCbValueAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                8888,
                7777,
                null,
                true,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task RecordCbValue_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.RecordCbValueAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                8888,
                7777,
                null,
                true,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task RecordCbValue_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.RecordCbValueAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                9999, 
                8888,
                7777,
                null,
                true,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task RecordCbValue_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.RecordCbValueAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess, 
                9999, 
                8888,
                7777,
                null,
                true,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task RecordCbValue_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.RecordCbValueAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                requirement.Id,
                requirement.Fields.First().Id,
                null,
                true,
                HttpStatusCode.BadRequest,
                "Tag and/or requirement doesn't exist!");
        }

        [TestMethod]
        public async Task RecordCbValue_AsPreserver_ShouldReturnBadRequest_WhenUnknownRequirementId()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.RecordCbValueAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                8888,
                requirement.Fields.First().Id,
                null,
                true,
                HttpStatusCode.BadRequest,
                "Tag and/or requirement doesn't exist!");
        }

        [TestMethod]
        public async Task RecordCbValue_AsPreserver_ShouldReturnBadRequest_WhenUnknownFieldId()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(UserType.Preserver, TestFactory.PlantWithAccess, tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.RecordCbValueAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest,
                requirement.Id,
                7777,
                null,
                true,
                HttpStatusCode.BadRequest,
                "Field doesn't exist in requirement!");
        }

        #endregion
        
        #region Transfer
        [TestMethod]
        public async Task Transfer_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.TransferAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                null,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Transfer_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.TransferAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Transfer_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.TransferAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Transfer_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.TransferAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Transfer_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.TransferAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Transfer_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.TransferAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Transfer_AsPlanner_ShouldReturnBadRequest_WhenIllegalRowVersion()
        {
            // Arrange 
            var tagResultDto = await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);
            var tagToTransfer = tagResultDto.Tags.FirstOrDefault(t => t.ReadyToBeTransferred);
            Assert.IsNotNull(tagToTransfer, "Bad test setup: Didn't find tag ready to be transferred");
            
            // Act
            await TagsControllerTestsHelper.TransferAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                new List<IdAndRowVersion>
                {
                    new IdAndRowVersion
                    {
                        Id = tagToTransfer.Id,
                        RowVersion = "invalidrowversion"
                    }
                },
                HttpStatusCode.BadRequest,
                "Not a valid row version!");
        }

        #endregion
        
        #region CompletePreservation
        [TestMethod]
        public async Task CompletePreservation_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.CompletePreservationAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                null,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CompletePreservation_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CompletePreservationAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CompletePreservation_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CompletePreservationAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CompletePreservation_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CompletePreservationAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CompletePreservation_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CompletePreservationAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CompletePreservation_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CompletePreservationAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CompletePreservation_AsPlanner_ShouldReturnBadRequest_WhenIllegalRowVersion()
        {
            // Arrange 
            var newReqDefId = await CreateRequirementDefinitionAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            var stepId = JourneyWithTags.Steps.Last().Id;

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
                null,
                null);
            await TagsControllerTestsHelper.StartPreservationAsync(UserType.Planner, TestFactory.PlantWithAccess, new List<int> {newTagId});

            var tagsResult = await TagsControllerTestsHelper.GetAllTagsAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);
            var tagToCompletedPreservation = tagsResult.Tags.Single(t => t.Id == newTagId);
            Assert.IsTrue(tagToCompletedPreservation.ReadyToBeCompleted, "Bad test setup: Didn't find tag ready to be completed");
            
            // Act
            await TagsControllerTestsHelper.CompletePreservationAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                new List<IdAndRowVersion>
                {
                    new IdAndRowVersion
                    {
                        Id = tagToCompletedPreservation.Id,
                        RowVersion = "invalidrowversion"
                    }
                },
                HttpStatusCode.BadRequest,
                "Not a valid row version!");
        }

        #endregion
        
        #region StartPreservation
        [TestMethod]
        public async Task StartPreservation_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.StartPreservationAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                null,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task StartPreservation_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.StartPreservationAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task StartPreservation_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.StartPreservationAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task StartPreservation_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.StartPreservationAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task StartPreservation_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.StartPreservationAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task StartPreservation_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.StartPreservationAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                null,
                HttpStatusCode.Forbidden);

        #endregion
        
        #region CreateAreaTag
        [TestMethod]
        public async Task CreateAreaTag_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.CreateAreaTagAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                TestFactory.ProjectWithAccess,
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                null,
                null,
                null,
                678,
                "Desc",
                null,
                null,
                null,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateAreaTag_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CreateAreaTagAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                TestFactory.ProjectWithAccess,
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                null,
                null,
                null,
                678,
                "Desc",
                null,
                null,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateAreaTag_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CreateAreaTagAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                TestFactory.ProjectWithAccess,
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                null,
                null,
                null,
                678,
                "Desc",
                null,
                null,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateAreaTag_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CreateAreaTagAsync(
                UserType.Hacker, TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess,
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                null,
                null,
                null,
                678,
                "Desc",
                null,
                null,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateAreaTag_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CreateAreaTagAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess, 
                TestFactory.ProjectWithAccess,
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                null,
                null,
                null,
                678,
                "Desc",
                null,
                null,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateAreaTag_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CreateAreaTagAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                TestFactory.ProjectWithAccess,
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                null,
                null,
                null,
                678,
                "Desc",
                null,
                null,
                null,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region GetHistory
        [TestMethod]
        public async Task GetHistory_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetHistoryAsync(
                UserType.Anonymous, 
                TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetHistory_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetHistoryAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetHistory_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetHistoryAsync(
                UserType.LibraryAdmin,
                TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetHistory_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetHistoryAsync(
                UserType.Hacker, 
                TestFactory.PlantWithAccess,
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetHistory_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetHistoryAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetHistory_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetHistoryAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                9999,
                HttpStatusCode.NotFound);

        #endregion
    }
}
