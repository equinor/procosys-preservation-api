using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Misc
{
    [TestClass]
    public class CrossPlantControllerNegativeTests : CrossPlantControllerTestsBase
    {
        #region GetAllTags
        [TestMethod]
        public async Task GetTags_AsAnonymous_ShouldReturnUnauthorized()
            => await CrossPlantControllerTestsHelper.GetTagsAsync(
                UserType.Anonymous,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetTags_AsHacker_ShouldReturnForbidden()
            => await CrossPlantControllerTestsHelper.GetTagsAsync(
                UserType.Hacker,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTags_AsAdmin_ShouldReturnForbidden()
            => await CrossPlantControllerTestsHelper.GetTagsAsync(
                UserType.LibraryAdmin,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTags_AsPreserver_ShouldReturnForbidden()
            => await CrossPlantControllerTestsHelper.GetTagsAsync(
                UserType.Preserver,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTags_AsPlanner_ShouldReturnForbidden()
            => await CrossPlantControllerTestsHelper.GetTagsAsync(
                UserType.Planner,
                HttpStatusCode.Forbidden);

        #endregion

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
