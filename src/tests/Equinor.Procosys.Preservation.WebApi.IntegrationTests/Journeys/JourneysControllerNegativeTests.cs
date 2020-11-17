using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Journeys
{
    [TestClass]
    public class JourneysControllerNegativeTests : JourneysControllerTestsBase
    {
        #region GetStep
        [TestMethod]
        public async Task GetStep_AsAnonymous_ShouldReturnUnauthorized()
            => await JourneysControllerTestsHelper.GetStepAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                8888,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetStep_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.GetStepAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetStep_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.GetStepAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                8888,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetStep_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.GetStepAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.GetStepAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.GetStepAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.GetStepAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                8888,
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
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateStep_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                8888,
                "Step1",
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
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                8888,
                "Step1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                "Step1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateStep_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await JourneysControllerTestsHelper.UpdateStepAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                8888,
                "Step1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        #endregion
    }
}
