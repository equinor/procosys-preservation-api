using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Misc
{
    [TestClass]
    public class CrossPlantControllerNegativeTests : CrossPlantControllerTestsBase
    {
        #region GetAllActions
        [TestMethod]
        public async Task GetActions_AsAnonymous_ShouldReturnUnauthorized()
            => await CrossPlantControllerTestsHelper.GetActionsAsync(
                UserType.Anonymous,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetActions_AsHacker_ShouldReturnForbidden()
            => await CrossPlantControllerTestsHelper.GetActionsAsync(
                UserType.Hacker, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetActions_AsAdmin_ShouldReturnForbidden()
            => await CrossPlantControllerTestsHelper.GetActionsAsync(
                UserType.LibraryAdmin, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetActions_AsPreserver_ShouldReturnForbidden()
            => await CrossPlantControllerTestsHelper.GetActionsAsync(
                UserType.Preserver, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetActions_AsPlanner_ShouldReturnForbidden()
            => await CrossPlantControllerTestsHelper.GetActionsAsync(
                UserType.Planner, 
                HttpStatusCode.Forbidden);

        #endregion
    }
}
