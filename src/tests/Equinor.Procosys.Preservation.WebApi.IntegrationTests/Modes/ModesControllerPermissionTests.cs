using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Modes
{
    [TestClass]
    public class ModesControllerPermissionTests : TestBase
    {
        #region GetAll
        [TestMethod]
        public async Task Get_AllModes_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Get_AllModes_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_AllModes_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_AllModes_AsHacker_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_AllModes_AsAdmin_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_AllModes_AsPlanner_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_AllModes_AsPreserver_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Get
        [TestMethod]
        public async Task Get_Mode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.GetModeAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Get_Mode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.GetModeAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_Mode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.GetModeAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999, 
                HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_Mode_AsHacker_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetModeAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_Mode_AsAdmin_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetModeAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess), 
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_Mode_AsAdmin_ShouldReturnNotFound()
            => await ModesControllerTestsHelper.GetModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                9999, 
                HttpStatusCode.NotFound);

        [TestMethod]
        public async Task Get_Mode_AsPlanner_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetModeAsync(
                PlannerClient(TestFactory.PlantWithAccess), 
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_Mode_AsPreserver_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetModeAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                9999, 
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Create
        [TestMethod]
        public async Task Create_Mode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.CreateModeAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                "Mode1",
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Create_Mode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.CreateModeAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                "Mode1",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Create_Mode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.CreateModeAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                "Mode1",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Create_Mode_AsHacker_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.CreateModeAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                "Mode1", 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Create_Mode_AsAdmin_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.CreateModeAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                "Mode1", 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Create_Mode_AsPlanner_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.CreateModeAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                "Mode1",
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Create_Mode_AsPreserver_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.CreateModeAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                "Mode1",
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Update
        [TestMethod]
        public async Task Update_Mode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Update_Mode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Update_Mode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Update_Mode_AsHacker_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Update_Mode_AsAdmin_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Update_Mode_AsPlanner_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Update_Mode_AsPreserver_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Void
        [TestMethod]
        public async Task Void_Mode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.VoidModeAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Void_Mode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.VoidModeAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Void_Mode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.VoidModeAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Void_Mode_AsHacker_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.VoidModeAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Void_Mode_AsAdmin_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.VoidModeAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Void_Mode_AsPlanner_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.VoidModeAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Void_Mode_AsPreserver_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.VoidModeAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Unvoid
        [TestMethod]
        public async Task Unvoid_Mode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Unvoid_Mode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Unvoid_Mode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Unvoid_Mode_AsHacker_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Unvoid_Mode_AsAdmin_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Unvoid_Mode_AsPlanner_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Unvoid_Mode_AsPreserver_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Delete
        [TestMethod]
        public async Task Delete_Mode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Delete_Mode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Delete_Mode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task Delete_Mode_AsHacker_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithoutAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Delete_Mode_AsAdmin_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                LibraryAdminClient(TestFactory.PlantWithoutAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Delete_Mode_AsPlanner_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Delete_Mode_AsPreserver_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        #endregion
    }
}
