using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Modes
{
    [TestClass]
    public class ModesControllerPermissionTests : TestBase
    {
        [TestMethod]
        public async Task Get_AllModes_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.GetAllModesAsync(AnonymousClient, TestFactory.UnknownPlant, HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Get_AllModes_AsHacker_ShouldReturnBadRequest()
            => await ModesControllerTestsHelper.GetAllModesAsync(AuthenticatedHackerClient, TestFactory.UnknownPlant, HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_AllModes_AsAdmin_ShouldReturnBadRequest()
            => await ModesControllerTestsHelper.GetAllModesAsync(LibraryAdminClient, TestFactory.UnknownPlant, HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_AllModes_AsHacker_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetAllModesAsync(AuthenticatedHackerClient, TestFactory.PlantWithoutAccess, HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_AllModes_AsAdmin_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetAllModesAsync(LibraryAdminClient, TestFactory.PlantWithoutAccess, HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_AllModes_AsAdmin_ShouldReturnOk()
            => await ModesControllerTestsHelper.GetAllModesAsync(LibraryAdminClient, TestFactory.PlantWithAccess);

        [TestMethod]
        public async Task Get_AllModes_AsPlanner_ShouldReturnOk()
            => await ModesControllerTestsHelper.GetAllModesAsync(PlannerClient, TestFactory.PlantWithAccess);

        [TestMethod]
        public async Task Get_AllModes_AsPreserver_ShouldReturnOk()
            => await ModesControllerTestsHelper.GetAllModesAsync(PreserverClient, TestFactory.PlantWithAccess);

        [TestMethod]
        public async Task Get_AllModes_AsAdmin_ShouldReturnEmptyList()
        {
            // Act
            var modes = await ModesControllerTestsHelper.GetAllModesAsync(LibraryAdminClient, TestFactory.PlantWithAccess);

            // Assert
            Assert.AreEqual(0, modes.Count);
        }
    }
}
