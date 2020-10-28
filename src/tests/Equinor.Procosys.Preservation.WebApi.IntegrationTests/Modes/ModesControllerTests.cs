using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Modes
{
    [TestClass]
    public class ModesControllerTests : TestBase
    {
        [TestMethod]
        public async Task Get_AllModes_AsAnonymous_ShouldReturnUnauthorized()
            => await ModesControllerTestsHelper.GetAllModesAsync(AnonymousClient, HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Get_AllModes_AsHacker_ShouldReturnForbidden()
            => await ModesControllerTestsHelper.GetAllModesAsync(HackerClient, HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_AllModes_AsAdmin_ShouldReturnOk()
            => await ModesControllerTestsHelper.GetAllModesAsync(LibraryAdminClient);

        [TestMethod]
        public async Task Get_AllModes_AsPlanner_ShouldReturnOk()
            => await ModesControllerTestsHelper.GetAllModesAsync(PlannerClient);

        [TestMethod]
        public async Task Get_AllModes_AsPreserver_ShouldReturnOk()
            => await ModesControllerTestsHelper.GetAllModesAsync(PreserverClient);

        [TestMethod]
        public async Task Get_AllModes_AsAdmin_ShouldReturnEmptyList()
        {
            // Act
            var modes = await ModesControllerTestsHelper.GetAllModesAsync(LibraryAdminClient);

            // Assert
            Assert.AreEqual(0, modes.Count);
        }
    }
}
