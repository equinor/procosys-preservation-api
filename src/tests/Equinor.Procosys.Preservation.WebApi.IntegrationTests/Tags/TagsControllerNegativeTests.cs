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
            var preserverClient = PreserverClient(TestFactory.PlantWithAccess);

            _tagId1_WithAttachment = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments;
            var attachmentDtos = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                preserverClient,
                _tagId1_WithAttachment);
            _tagAttachmentId1 = attachmentDtos.First().Id;

            _tagId2_WithAttachment = TagIdUnderTest_ForSiteAreaTagWithAttachmentsAndActionAttachments;
            attachmentDtos = await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                preserverClient,
                _tagId2_WithAttachment);
            _tagAttachmentId2 = attachmentDtos.First().Id;

            _tagId1_WithAction = TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments;
            var actionDtos = await TagsControllerTestsHelper.GetAllActionsAsync(
                preserverClient,
                _tagId1_WithAction);
            _tagActionId1 = actionDtos.First().Id;

            _tagId2_WithAction = TagIdUnderTest_ForSiteAreaTagWithAttachmentsAndActionAttachments;
            actionDtos = await TagsControllerTestsHelper.GetAllActionsAsync(
                preserverClient,
                _tagId2_WithAction);
            _tagActionId2 = actionDtos.First().Id;

            var actionAttachmentDtos = await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                preserverClient,
                _tagId1_WithAction,
                _tagActionId1);
            _tagActionAttachmentId1 = actionAttachmentDtos.First().Id;

            actionAttachmentDtos = await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                preserverClient,
                _tagId2_WithAction,
                _tagActionId2);
            _tagActionAttachmentId2 = actionAttachmentDtos.First().Id;

        }

        #region GetAllTags
        [TestMethod]
        public async Task GetAllTags_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                TestFactory.ProjectWithAccess,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllTags_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                TestFactory.ProjectWithAccess,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTags_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                TestFactory.ProjectWithAccess,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTags_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllTags_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region GetTag
        [TestMethod]
        public async Task GetTag_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetTagAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetTag_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetTagAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetTag_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetTagAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999, 
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetTag_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetTagAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTag_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetTagAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTag_AsPlanner_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetTagAsync(
                PlannerClient(TestFactory.PlantWithAccess), 
                9999, 
                HttpStatusCode.NotFound);

        [TestMethod]
        public async Task GetTag_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetTagAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                9999, 
                HttpStatusCode.NotFound);
        #endregion
        
        #region DuplicateAreaTag
        [TestMethod]
        public async Task DuplicateAreaTag_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                AnonymousClient(TestFactory.UnknownPlant),
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
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
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
                LibraryAdminClient(TestFactory.UnknownPlant),
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
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
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
                LibraryAdminClient(TestFactory.PlantWithAccess), 
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
                PreserverClient(TestFactory.PlantWithAccess), 
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
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                null,
                1111,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                null,
                1111,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"is not a valid plant");

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                null,
                1111,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.BadRequest,
                expectedMessageOnBadRequest:"is not a valid plant");

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999,
                null,
                5555,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                9999,
                null,
                5555,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                null,
                5555,
                TestFactory.AValidRowVersion,
                expectedStatusCode:HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPlanner_ShouldReturnBadRequest_WhenChangeDescriptionOnStandardTag()
        {
            // Arrange
            var plannerClient = PlannerClient(TestFactory.PlantWithAccess);
            var tag = await TagsControllerTestsHelper.GetTagAsync(
                plannerClient, 
                _tagId1_WithAttachment);
            var oldDescription = tag.Description;
            var newDescription = Guid.NewGuid().ToString();
            Assert.AreNotEqual(oldDescription, newDescription);

            // Act
            await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                plannerClient,
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
                AnonymousClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllTagAttachments_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTagAttachments_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTagAttachments_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                _tagId1_WithAttachment,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllTagAttachments_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                _tagId1_WithAttachment,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllTagAttachments_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                HttpStatusCode.NotFound);

        #endregion

        #region DeleteTagAttachment
        [TestMethod]
        public async Task DeleteTagAttachment_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                _tagAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteTagAttachment_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                _tagAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteTagAttachment_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                _tagAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteTagAttachment_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999,
                5555,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteTagAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                9999,
                5555,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteTagAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                _tagAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag and/or attachment doesn't exist!");

        [TestMethod]
        public async Task DeleteTagAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownAttachmentId()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
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
                AnonymousClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                _tagActionId1,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllActionAttachments_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                _tagActionId1,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActionAttachments_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                _tagActionId1,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActionAttachments_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999,
                5555,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActionAttachments_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                9999,
                5555,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActionAttachments_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                _tagActionId2,
                HttpStatusCode.NotFound);

        [TestMethod]
        public async Task GetAllActionAttachments_AsPreserver_ShouldReturnNotFound_WhenUnknownActionId()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                _tagId1_WithAttachment,
                _tagActionId2, // known actionId, but under other tag
                HttpStatusCode.NotFound);

        #endregion

        #region DeleteActionAttachment
        [TestMethod]
        public async Task DeleteActionAttachment_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                _tagActionId1,
                _tagActionAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteActionAttachment_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                _tagActionId1,
                _tagActionAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteActionAttachment_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                _tagActionId1,
                _tagActionAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteActionAttachment_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999,
                5555,
                4444,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteActionAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                9999,
                5555,
                4444,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                _tagActionId1,
                _tagActionAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag, action and/or attachment doesn't exist!");

        [TestMethod]
        public async Task DeleteActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownActionId()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                _tagId1_WithAttachment,
                _tagActionId2, // known actionId, but under other tag
                _tagActionAttachmentId1,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag, action and/or attachment doesn't exist!");

        [TestMethod]
        public async Task DeleteActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownAttachmentId()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
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
                AnonymousClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllActions_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActions_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                _tagId1_WithAttachment,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActions_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActions_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActions_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                HttpStatusCode.NotFound);

        #endregion
        
        #region UpdateAction
        [TestMethod]
        public async Task UpdateAction_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                null,
                null,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateAction_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
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
                LibraryAdminClient(TestFactory.UnknownPlant),
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
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999, 
                8888,
                null,
                null,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                9999, 
                8888,
                null,
                null,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
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
                PreserverClient(TestFactory.PlantWithAccess),
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
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAction_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetActionAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAction_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetActionAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999, 
                8888,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAction_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetActionAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetActionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                9999, 
                8888,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAction_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetActionAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                9999, 
                _tagActionId2,
                HttpStatusCode.NotFound);

        [TestMethod]
        public async Task GetAction_AsPreserver_ShouldReturnNotFound_WhenUnknownActionId()
            => await TagsControllerTestsHelper.GetActionAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                _tagActionId1,   // known actionId, but under other Tag
                HttpStatusCode.NotFound);
        #endregion
        
        #region CloseAction
        [TestMethod]
        public async Task CloseAction_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.CloseActionAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CloseAction_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CloseActionAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CloseAction_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CloseActionAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CloseAction_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CloseActionAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999, 
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CloseAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CloseActionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                9999, 
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task CloseAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.CloseActionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999, 
                _tagActionId2,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag and/or action doesn't exist!");

        [TestMethod]
        public async Task CloseAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownActionId()
            => await TagsControllerTestsHelper.CloseActionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
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
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                FileToBeUploaded,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UploadActionAttachment_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UploadActionAttachment_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UploadActionAttachment_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999, 
                8888,
                FileToBeUploaded,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UploadActionAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                9999, 
                8888,
                FileToBeUploaded,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task UploadActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999, 
                _tagActionId2,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "Tag and/or action doesn't exist!");

        [TestMethod]
        public async Task UploadActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownActionId()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
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
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                null,
                null,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateAction_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CreateActionAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                null,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateAction_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CreateActionAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                null,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateAction_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CreateActionAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999, 
                null,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CreateActionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                9999, 
                null,
                null,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task CreateAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.CreateActionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
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
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetTagRequirementsAsync_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetTagRequirementsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetTagRequirementsAsync_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetTagRequirementsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999, 
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetTagRequirementsAsync_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetTagRequirementsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTagRequirements_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetTagRequirementsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTagRequirements_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetTagRequirementsAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                9999, 
                HttpStatusCode.NotFound);
        #endregion
        
        #region UploadFieldValueAttachment
        [TestMethod]
        public async Task UploadFieldValueAttachment_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                7777,
                FileToBeUploaded,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UploadFieldValueAttachment_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                7777,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UploadFieldValueAttachment_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                7777,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UploadFieldValueAttachment_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999, 
                8888,
                7777,
                FileToBeUploaded,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UploadFieldValueAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                9999, 
                8888,
                7777,
                FileToBeUploaded,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task UploadFieldValueAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
        {
            // Arrange
            var client = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(client, tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
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
            var client = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(client, tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
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
            var client = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(client, tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
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
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task PreserveRequirement_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.PreserveRequirementAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task PreserveRequirement_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.PreserveRequirementAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task PreserveRequirement_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.PreserveRequirementAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task PreserveRequirement_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.PreserveRequirementAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                9999,
                8888,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task PreserveRequirement_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
        {
            // Arrange
            var client = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(client, tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.PreserveRequirementAsync(
                client,
                9999,
                requirement.Id,
                HttpStatusCode.BadRequest,
                "Tag and/or requirement doesn't exist!");
        }

        [TestMethod]
        public async Task PreserveRequirement_AsPreserver_ShouldReturnBadRequest_WhenUnknownRequirementId()
            => await TagsControllerTestsHelper.PreserveRequirementAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started,
                8888,
                HttpStatusCode.BadRequest,
                "Tag and/or requirement doesn't exist!");

        #endregion
        
        #region RecordCbValue
        [TestMethod]
        public async Task RecordCbValue_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.RecordCbValueAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                7777,
                null,
                true,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task RecordCbValue_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.RecordCbValueAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
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
                LibraryAdminClient(TestFactory.UnknownPlant),
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
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                9999, 
                8888,
                7777,
                null,
                true,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task RecordCbValue_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.RecordCbValueAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
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
            var client = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(client, tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.RecordCbValueAsync(
                PreserverClient(TestFactory.PlantWithAccess),
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
            var client = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(client, tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.RecordCbValueAsync(
                PreserverClient(TestFactory.PlantWithAccess),
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
            var client = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForStandardTagWithAttachmentRequirement_Started;
            var requirement = await TagsControllerTestsHelper.GetTagRequirementInfoAsync(client, tagIdUnderTest);

            // Act
            await TagsControllerTestsHelper.RecordCbValueAsync(
                PreserverClient(TestFactory.PlantWithAccess),
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
                AnonymousClient(TestFactory.UnknownPlant),
                null,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Transfer_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.TransferAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Transfer_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.TransferAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Transfer_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.TransferAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Transfer_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.TransferAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Transfer_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.TransferAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Transfer_AsPlanner_ShouldReturnBadRequest_WhenIllegalRowVersion()
        {
            // Arrange 
            var plannerClient = PlannerClient(TestFactory.PlantWithAccess);
            var tagResultDto = await TagsControllerTestsHelper.GetAllTagsAsync(
                plannerClient,
                TestFactory.ProjectWithAccess);
            var tagToTransfer = tagResultDto.Tags.FirstOrDefault(t => t.ReadyToBeTransferred);
            Assert.IsNotNull(tagToTransfer, "Bad test setup: Didn't find tag ready to be transferred");
            
            // Act
            await TagsControllerTestsHelper.TransferAsync(
                plannerClient,
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
                AnonymousClient(TestFactory.UnknownPlant),
                null,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CompletePreservation_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CompletePreservationAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CompletePreservation_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CompletePreservationAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CompletePreservation_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CompletePreservationAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CompletePreservation_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CompletePreservationAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CompletePreservation_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CompletePreservationAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CompletePreservation_AsPlanner_ShouldReturnBadRequest_WhenIllegalRowVersion()
        {
            // Arrange 
            var client = LibraryAdminClient(TestFactory.PlantWithAccess);
            var newReqDefId = await CreateRequirementDefinitionAsync(client);
            var stepId = JourneyWithTags.Steps.Last().Id;

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
            var tagToCompletedPreservation = tagsResult.Tags.Single(t => t.Id == newTagId);
            Assert.IsTrue(tagToCompletedPreservation.ReadyToBeCompleted, "Bad test setup: Didn't find tag ready to be completed");
            
            // Act
            await TagsControllerTestsHelper.CompletePreservationAsync(
                client,
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
                AnonymousClient(TestFactory.UnknownPlant),
                null,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task StartPreservation_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.StartPreservationAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task StartPreservation_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.StartPreservationAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task StartPreservation_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.StartPreservationAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task StartPreservation_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.StartPreservationAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task StartPreservation_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.StartPreservationAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                null,
                HttpStatusCode.Forbidden);

        #endregion
        
        #region CreateAreaTag
        [TestMethod]
        public async Task CreateAreaTag_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.CreateAreaTagAsync(
                AnonymousClient(TestFactory.UnknownPlant),
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
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateAreaTag_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CreateAreaTagAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
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
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateAreaTag_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.CreateAreaTagAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
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
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateAreaTag_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CreateAreaTagAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
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
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateAreaTag_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CreateAreaTagAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
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
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateAreaTag_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CreateAreaTagAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
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
                HttpStatusCode.Forbidden);
        #endregion
    }
}
