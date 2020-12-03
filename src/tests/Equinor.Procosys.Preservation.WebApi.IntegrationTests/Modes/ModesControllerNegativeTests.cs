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
                UserType.Anonymous, TestFactory.UnknownPlant,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllModes_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllModes_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllModes_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllModes_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllModes_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllModes_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.GetAllModesAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Get
        [TestMethod]
        public async Task GetMode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.GetModeAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetMode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.GetModeAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetMode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.GetModeAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999, 
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetMode_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.GetModeAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetMode_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.GetModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess, 
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetMode_AsAdmin_ShouldReturnNotFound()
            => await ModesControllerTestsHelper.GetModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess, 
                9999, 
                HttpStatusCode.NotFound);

        [TestMethod]
        public async Task GetMode_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.GetModeAsync(
                UserType.Planner, TestFactory.PlantWithAccess, 
                9999, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetMode_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.GetModeAsync(
                UserType.Preserver, TestFactory.PlantWithAccess, 
                9999, 
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Create
        [TestMethod]
        public async Task CreateMode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.CreateModeAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                "Mode1",
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateMode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.CreateModeAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                "Mode1",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateMode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.CreateModeAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                "Mode1",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateMode_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.CreateModeAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                "Mode1", 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateMode_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.CreateModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                "Mode1", 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateMode_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.CreateModeAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                "Mode1",
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateMode_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.CreateModeAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                "Mode1",
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Update
        [TestMethod]
        public async Task UpdateMode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateMode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateMode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateMode_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateMode_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateMode_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateMode_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.UpdateModeAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                "Mode1",
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Void
        [TestMethod]
        public async Task VoidMode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.VoidModeAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task VoidMode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.VoidModeAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidMode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.VoidModeAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task VoidMode_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.VoidModeAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidMode_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.VoidModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidMode_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.VoidModeAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task VoidMode_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.VoidModeAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Unvoid
        [TestMethod]
        public async Task UnvoidMode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UnvoidMode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidMode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UnvoidMode_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidMode_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidMode_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UnvoidMode_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.UnvoidModeAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Delete
        [TestMethod]
        public async Task DeleteMode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                UserType.Anonymous, TestFactory.UnknownPlant,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteMode_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                UserType.Hacker, TestFactory.UnknownPlant,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteMode_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                UserType.LibraryAdmin, TestFactory.UnknownPlant,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteMode_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                UserType.Hacker, TestFactory.PlantWithoutAccess,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteMode_AsAdmin_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithoutAccess,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteMode_AsPlanner_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                UserType.Planner, TestFactory.PlantWithAccess,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteMode_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await ModesControllerTestsHelper.DeleteModeAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                9999,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);
        #endregion
    }
}
