using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Journeys
{
    [TestClass]
    public class JourneysControllerNegativeTests : JourneysControllerTestsBase
    {
        #region GetJourneys
        [TestMethod]
        public async Task GetJourneys_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetJourneys_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetJourneys_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetJourneys_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourneys_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourneys_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourneys_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.GetJourneysAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                HttpStatusCode.Forbidden);
        #endregion

        #region GetJourney
        [TestMethod]
        public async Task GetJourney_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetJourney_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetJourney_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetJourney_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourney_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourney_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetJourney_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.GetJourneyAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                HttpStatusCode.Forbidden);
        #endregion

        #region CreateStep
        [TestMethod]
        public async Task CreateStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateStep_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.CreateStepAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                "Step1",
                7777,
                "RC",
                HttpStatusCode.Forbidden);
        #endregion

        #region UpdateStep
        [TestMethod]
        public async Task UpdateStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateStep_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                "Step1",
                7777,
                "RC",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownJourneyOrStepId()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyNotInUseIdUnderTest,
                StepInJourneyWithTagsIdUnderTest, // step in other Journey
                Guid.NewGuid().ToString(),
                ModeIdUnderTest,
                KnownTestData.ResponsibleCode,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Journey and/or step doesn't exist!");

        #endregion

        #region VoidStep
        [TestMethod]
        public async Task VoidStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task VoidStep_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownJourneyOrStepId()
            => await JourneysControllerTestsHelper.VoidStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyNotInUseIdUnderTest,
                StepInJourneyWithTagsIdUnderTest, // step in other Journey
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Journey and/or step doesn't exist!");
        #endregion

        #region UnvoidStep
        [TestMethod]
        public async Task UnvoidStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UnvoidStep_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownJourneyOrStepId()
            => await JourneysControllerTestsHelper.UnvoidStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyNotInUseIdUnderTest,
                StepInJourneyWithTagsIdUnderTest, // step in other Journey
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Journey and/or step doesn't exist!");
        #endregion

        #region DeleteStep
        [TestMethod]
        public async Task DeleteStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteStep_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownJourneyOrStepId()
            => await JourneysControllerTestsHelper.DeleteStepAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                JourneyNotInUseIdUnderTest,
                StepInJourneyWithTagsIdUnderTest, // step in other Journey
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "Journey and/or step doesn't exist!");

        #endregion
    }
}
