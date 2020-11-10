using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Modes
{
    [TestClass]
    public class ModesControllerNegativeTests : TestBase
    {
        #region GetAll
        [TestMethod]
        public async Task GetAllModes_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllModes_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllModes_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllModes_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllModes_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllModes_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllModes_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Get
        [TestMethod]
        public async Task GetMode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.GetModeAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetMode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.GetModeAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetMode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.GetModeAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999, 
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetMode_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.GetModeAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetMode_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.GetModeAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess), 
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetMode_AsAdmin_ShouldReturnNotFound()
            => await ModesControllerTestsHelper.GetModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                9999, 
                HttpStatusCode.NotFound);

        [TestMethod]
        public async Task GetMode_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.GetModeAsync(
                PlannerClient(TestFactory.PlantWithAccess), 
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetMode_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.GetModeAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                9999, 
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Create
        [TestMethod]
        public async Task CreateMode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.CreateModeAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                "Mode1",
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateMode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.CreateModeAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                "Mode1",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateMode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.CreateModeAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                "Mode1",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateMode_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.CreateModeAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                "Mode1", 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateMode_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.CreateModeAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                "Mode1", 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateMode_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.CreateModeAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                "Mode1",
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateMode_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.CreateModeAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                "Mode1",
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Update
        [TestMethod]
        public async Task UpdateMode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateMode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateMode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateMode_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateMode_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateMode_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateMode_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Void
        [TestMethod]
        public async Task VoidMode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.VoidModeAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task VoidMode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.VoidModeAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidMode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.VoidModeAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidMode_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.VoidModeAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidMode_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.VoidModeAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidMode_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.VoidModeAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidMode_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.VoidModeAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Unvoid
        [TestMethod]
        public async Task UnvoidMode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UnvoidMode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidMode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidMode_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidMode_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidMode_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidMode_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Delete
        [TestMethod]
        public async Task DeleteMode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteMode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteMode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteMode_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteMode_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteMode_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteMode_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        #endregion
    }
}
