using System;
using System.Net;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    [TestClass]
    public class TagsControllerNegativeTests : TagsControllerTestsBase
    {
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
                SiteAreaTagIdUnderTest, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTag_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetTagAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                SiteAreaTagIdUnderTest, 
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
                SiteAreaTagIdUnderTest, 
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
                SiteAreaTagIdUnderTest, 
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
                SiteAreaTagIdUnderTest, 
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
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                null,
                1111,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                null,
                1111,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                SiteAreaTagIdUnderTest, 
                null,
                StepIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                SiteAreaTagIdUnderTest, 
                null,
                StepIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                SiteAreaTagIdUnderTest, 
                null,
                StepIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPlanner_ShouldReturnBadRequest_WhenChangeDescriptionOnStandardTag()
        {
            // Arrange
            var plannerClient = PlannerClient(TestFactory.PlantWithAccess);
            var tag = await TagsControllerTestsHelper.GetTagAsync(
                plannerClient, 
                StandardTagIdUnderTest);
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
                HttpStatusCode.BadRequest,
                "Tag must be an area tag to update description!");
        }
        #endregion

        #region GetAllTagAttachments
        [TestMethod]
        public async Task GetAllTagAttachments_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllTagAttachments_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTagAttachments_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTagAttachments_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllTagAttachments_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
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
                StandardTagIdUnderTest,
                StandardTagAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteTagAttachment_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                StandardTagAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteTagAttachment_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                StandardTagAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteTagAttachment_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
                StandardTagAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteTagAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
                StandardTagAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteTagAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                StandardTagAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag and/or attachment doesn't exist!");

        [TestMethod]
        public async Task DeleteTagAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownAttachmentId()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
                SiteAreaTagAttachmentIdUnderTest,  // known attachmentId, but under other Tag
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag and/or attachment doesn't exist!");
        #endregion
        
        #region GetAllActionAttachments
        [TestMethod]
        public async Task GetAllActionAttachments_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllActionAttachments_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActionAttachments_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActionAttachments_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActionAttachments_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActionAttachments_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                SiteAreaTagActionIdUnderTest,
                HttpStatusCode.NotFound);

        [TestMethod]
        public async Task GetAllActionAttachments_AsPreserver_ShouldReturnNotFound_WhenUnknownActionId()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
                SiteAreaTagActionIdUnderTest, // known actionId, but under other tag
                HttpStatusCode.NotFound);

        #endregion

        #region DeleteActionAttachment
        [TestMethod]
        public async Task DeleteActionAttachment_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest,
                StandardTagActionAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteActionAttachment_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest,
                StandardTagActionAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteActionAttachment_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest,
                StandardTagActionAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteActionAttachment_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest,
                StandardTagActionAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteActionAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest,
                StandardTagActionAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                StandardTagActionIdUnderTest,
                StandardTagActionAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag, action and/or attachment doesn't exist!");

        [TestMethod]
        public async Task DeleteActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownActionId()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
                SiteAreaTagActionIdUnderTest, // known actionId, but under other tag
                StandardTagActionAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag, action and/or attachment doesn't exist!");

        [TestMethod]
        public async Task DeleteActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownAttachmentId()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
                StandardTagActionIdUnderTest,
                SiteAreaTagActionAttachmentIdUnderTest,  // known attachmentId, but under other action
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag, action and/or attachment doesn't exist!");

        #endregion
        
        #region GetAllActions
        [TestMethod]
        public async Task GetAllActions_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllActions_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActions_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                StandardTagIdUnderTest,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActions_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActions_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                StandardTagIdUnderTest,
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
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateAction_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                SiteAreaTagIdUnderTest, 
                SiteAreaTagActionIdUnderTest,
                null,
                null,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                SiteAreaTagIdUnderTest, 
                SiteAreaTagActionIdUnderTest,
                null,
                null,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task UpdateAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999, 
                SiteAreaTagActionIdUnderTest,
                "TestTitle",
                "TestDescription",
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag and/or action doesn't exist!");

        [TestMethod]
        public async Task UpdateAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownAttachmentId()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                SiteAreaTagIdUnderTest, 
                StandardTagActionIdUnderTest,   // known actionId, but under other Tag
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
                SiteAreaTagIdUnderTest,
                SiteAreaTagActionIdUnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetActionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                SiteAreaTagIdUnderTest, 
                SiteAreaTagActionIdUnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAction_AsPreserver_ShouldReturnNotFound_WhenUnknownTagId()
            => await TagsControllerTestsHelper.GetActionAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                9999, 
                SiteAreaTagActionIdUnderTest,
                HttpStatusCode.NotFound);

        [TestMethod]
        public async Task GetAction_AsPreserver_ShouldReturnNotFound_WhenUnknownActionId()
            => await TagsControllerTestsHelper.GetActionAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                SiteAreaTagIdUnderTest, 
                StandardTagActionIdUnderTest,   // known actionId, but under other Tag
                HttpStatusCode.NotFound);
        #endregion
    }
}
