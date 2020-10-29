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
            => await ModesControllerTestsHelper.GetAllModesAsync(AnonymousClient(TestFactory.UnknownPlant), HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Get_AllModes_AsHacker_ShouldReturnBadRequest()
            => await ModesControllerTestsHelper.GetAllModesAsync(AuthenticatedHackerClient(TestFactory.UnknownPlant), HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_AllModes_AsAdmin_ShouldReturnBadRequest()
            => await ModesControllerTestsHelper.GetAllModesAsync(LibraryAdminClient(TestFactory.UnknownPlant), HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_AllModes_AsHacker_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetAllModesAsync(AuthenticatedHackerClient(TestFactory.PlantWithoutAccess), HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_AllModes_AsAdmin_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetAllModesAsync(LibraryAdminClient(TestFactory.PlantWithoutAccess), HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_AllModes_AsAdmin_ShouldReturnOk()
            => await ModesControllerTestsHelper.GetAllModesAsync(LibraryAdminClient(TestFactory.PlantWithAccess));

        [TestMethod]
        public async Task Get_AllModes_AsPlanner_ShouldReturnOk()
            => await ModesControllerTestsHelper.GetAllModesAsync(PlannerClient(TestFactory.PlantWithAccess));

        [TestMethod]
        public async Task Get_AllModes_AsPreserver_ShouldReturnOk()
            => await ModesControllerTestsHelper.GetAllModesAsync(PreserverClient(TestFactory.PlantWithAccess));
        #endregion
        
        #region Get
        [TestMethod]
        public async Task Get_Mode_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.GetModeAsync(AnonymousClient(TestFactory.UnknownPlant), 999, HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Get_Mode_AsHacker_ShouldReturnBadRequest()
            => await ModesControllerTestsHelper.GetModeAsync(AuthenticatedHackerClient(TestFactory.UnknownPlant), 999, HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_Mode_AsAdmin_ShouldReturnBadRequest()
            => await ModesControllerTestsHelper.GetModeAsync(LibraryAdminClient(TestFactory.UnknownPlant), 999, HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_Mode_AsHacker_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetModeAsync(AuthenticatedHackerClient(TestFactory.PlantWithoutAccess), 999, HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_Mode_AsAdmin_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetModeAsync(LibraryAdminClient(TestFactory.PlantWithoutAccess), 999, HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_Mode_AsAdmin_ShouldReturnNotFound()
            => await ModesControllerTestsHelper.GetModeAsync(LibraryAdminClient(TestFactory.PlantWithAccess), 999, HttpStatusCode.NotFound);

        [TestMethod]
        public async Task Get_Mode_AsPlanner_ShouldReturnNotFound()
            => await ModesControllerTestsHelper.GetModeAsync(PlannerClient(TestFactory.PlantWithAccess), 999, HttpStatusCode.NotFound);

        [TestMethod]
        public async Task Get_Mode_AsPreserver_ShouldReturnNotFound()
            => await ModesControllerTestsHelper.GetModeAsync(PreserverClient(TestFactory.PlantWithAccess), 999, HttpStatusCode.NotFound);
        #endregion
    }
}
