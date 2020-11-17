using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Journeys
{
    [TestClass]
    public class JourneysControllerNegativeTests : JourneysControllerTestsBase
    {
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
        #endregion
    }
}
