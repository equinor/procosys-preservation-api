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
        public async Task GetAllActions_AsAnonymous_ShouldReturnUnauthorized()
            => await CrossPlantControllerTestsHelper.GetAllActionsAsync(
                UserType.Anonymous,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllActions_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await CrossPlantControllerTestsHelper.GetAllActionsAsync(
                UserType.Hacker, 
                HttpStatusCode.Forbidden,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActions_AsAdmin_ShouldReturnForbidden_WhenUnknownPlant()
            => await CrossPlantControllerTestsHelper.GetAllActionsAsync(
                UserType.LibraryAdmin, 
                HttpStatusCode.Forbidden,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllActions_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await CrossPlantControllerTestsHelper.GetAllActionsAsync(
                UserType.Hacker, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllActions_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await CrossPlantControllerTestsHelper.GetAllActionsAsync(
                UserType.LibraryAdmin, 
                HttpStatusCode.Forbidden);

        #endregion
    }
}
