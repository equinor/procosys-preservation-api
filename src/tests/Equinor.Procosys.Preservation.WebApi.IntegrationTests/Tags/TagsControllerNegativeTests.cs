﻿using System;
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
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTag_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetTagAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
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
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
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
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
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
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
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
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted;
            var currentStepId = await GetCurrentStepId(PreserverClient(TestFactory.PlantWithAccess), tagIdUnderTest);
            
            // Act
            await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                tagIdUnderTest,
                null,
                currentStepId,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
        {
            // Arrange
            var tagIdUnderTest = TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted;
            var currentStepId = await GetCurrentStepId(PreserverClient(TestFactory.PlantWithAccess), tagIdUnderTest);
            
            // Act
            await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                tagIdUnderTest,
                null,
                currentStepId,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
        {
            // Arrange
            var client = PreserverClient(TestFactory.PlantWithAccess);
            var tagIdUnderTest = TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted;
            var currentStepId = await GetCurrentStepId(client, tagIdUnderTest);
            
            // Act
            await TagsControllerTestsHelper.UpdateTagStepAndRequirementsAsync(
                client,
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted,
                null,
                currentStepId,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task UpdateTagStepAndRequirements_AsPlanner_ShouldReturnBadRequest_WhenChangeDescriptionOnStandardTag()
        {
            // Arrange
            var plannerClient = PlannerClient(TestFactory.PlantWithAccess);
            var tag = await TagsControllerTestsHelper.GetTagAsync(
                plannerClient, 
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted);
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
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllTagAttachments_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTagAttachments_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTagAttachments_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllTagAttachments_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagAttachmentsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
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
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                StandardTagAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteTagAttachment_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                StandardTagAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteTagAttachment_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                StandardTagAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteTagAttachment_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                StandardTagAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteTagAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteTagAttachmentAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
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
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
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
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                StandardTagActionIdUnderTest,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllActionAttachments_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                StandardTagActionIdUnderTest,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActionAttachments_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                StandardTagActionIdUnderTest,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActionAttachments_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                StandardTagActionIdUnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActionAttachments_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionAttachmentsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
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
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                SiteAreaTagActionIdUnderTest, // known actionId, but under other tag
                HttpStatusCode.NotFound);

        #endregion

        #region DeleteActionAttachment
        [TestMethod]
        public async Task DeleteActionAttachment_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                StandardTagActionIdUnderTest,
                StandardTagActionAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteActionAttachment_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                StandardTagActionIdUnderTest,
                StandardTagActionAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteActionAttachment_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                StandardTagActionIdUnderTest,
                StandardTagActionAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteActionAttachment_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                StandardTagActionIdUnderTest,
                StandardTagActionAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteActionAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
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
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                SiteAreaTagActionIdUnderTest, // known actionId, but under other tag
                StandardTagActionAttachmentIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag, action and/or attachment doesn't exist!");

        [TestMethod]
        public async Task DeleteActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownAttachmentId()
            => await TagsControllerTestsHelper.DeleteActionAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
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
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllActions_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActions_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActions_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActions_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllActionsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForStandardTagReadyForBulkPreserve_NotStarted,
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
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                SiteAreaTagActionIdUnderTest,
                null,
                null,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
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
        public async Task UpdateAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownActionId()
            => await TagsControllerTestsHelper.UpdateActionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
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
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted,
                SiteAreaTagActionIdUnderTest,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetActionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
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
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                StandardTagActionIdUnderTest,   // known actionId, but under other Tag
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
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                SiteAreaTagActionIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CloseAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CloseActionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                SiteAreaTagActionIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task CloseAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.CloseActionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999, 
                SiteAreaTagActionIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Tag and/or action doesn't exist!");

        [TestMethod]
        public async Task CloseAction_AsPreserver_ShouldReturnBadRequest_WhenUnknownActionId()
            => await TagsControllerTestsHelper.CloseActionAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                StandardTagActionIdUnderTest,   // known actionId, but under other Tag
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
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                SiteAreaTagActionIdUnderTest,
                FileToBeUploaded,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UploadActionAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                SiteAreaTagActionIdUnderTest,
                FileToBeUploaded,
                HttpStatusCode.Forbidden);


        [TestMethod]
        public async Task UploadActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownTagId()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999, 
                SiteAreaTagActionIdUnderTest,
                FileToBeUploaded,
                HttpStatusCode.BadRequest,
                "Tag and/or action doesn't exist!");

        [TestMethod]
        public async Task UploadActionAttachment_AsPreserver_ShouldReturnBadRequest_WhenUnknownActionId()
            => await TagsControllerTestsHelper.UploadActionAttachmentAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                StandardTagActionIdUnderTest,   // known actionId, but under other Tag
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
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                null,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateAction_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.CreateActionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
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
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTagRequirements_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetTagRequirementsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
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
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                SiteAreaTagActionIdUnderTest,
                7777,
                FileToBeUploaded,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UploadFieldValueAttachment_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.UploadFieldValueAttachmentAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                TagIdUnderTest_ForSiteAreaTagReadyForBulkPreserve_NotStarted, 
                SiteAreaTagActionIdUnderTest,
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
                "Tag, requirement and/or field action doesn't exist!");
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
                "Tag, requirement and/or field doesn't exist!");
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
                "Tag, requirement and/or field doesn't exist!");
        }

        #endregion
    }
}
