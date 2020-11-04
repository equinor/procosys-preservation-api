using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    [TestClass]
    public class TagsControllerPermissionTests : TestBase
    {
        private int initialTagId;

        [TestInitialize]
        public async Task TestInitialize()
        {
            var result = await TagsControllerTestsHelper.GetAllTagsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess);

            Assert.IsNotNull(result);

            Assert.IsTrue(result.Tags.Count > 0, "Didn't find any tags at startup. Bad test setup");
            initialTagId = result.Tags.First().Id;
        }

        #region GetAll
        [TestMethod]
        public async Task Get_AllTags_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                TestFactory.ProjectWithAccess,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Get_AllTags_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                TestFactory.ProjectWithAccess,
                HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_AllTags_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                TestFactory.ProjectWithAccess,
                HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_AllTags_AsHacker_ShouldReturnForbidden()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess,
                // Forbidden due to lack of permission
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_AllTags_AsAdmin_ShouldReturnForbidden()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess,
                // Forbidden due to lack of permission
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_AllTags_AsPlanner_ShouldReturnOk()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess);

        [TestMethod]
        public async Task Get_AllTags_AsPreserver_ShouldReturnOk()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess);
        #endregion
        
        #region Get
        [TestMethod]
        public async Task Get_Tag_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetTagAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task Get_Tag_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetTagAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_Tag_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetTagAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999, 
                HttpStatusCode.BadRequest);

        [TestMethod]
        public async Task Get_Tag_AsHacker_ShouldReturnForbidden()
            => await TagsControllerTestsHelper.GetTagAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                initialTagId, 
                // Forbidden due to lack of permission
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_Tag_AsAdmin_ShouldReturnForbidden()
            => await TagsControllerTestsHelper.GetTagAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                initialTagId, 
                // Forbidden due to lack of permission
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_Tag_AsPlanner_ShouldReturnOK_WhenKnownId()
            => await TagsControllerTestsHelper.GetTagAsync(
                PlannerClient(TestFactory.PlantWithAccess), 
                initialTagId);

        [TestMethod]
        public async Task Get_Tag_AsPreserver_ShouldReturnOK_WhenKnownId()
            => await TagsControllerTestsHelper.GetTagAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                initialTagId);

        [TestMethod]
        public async Task Get_Tag_AsPlanner_ShouldReturnForbidden_WhenUnknownId()
            => await TagsControllerTestsHelper.GetTagAsync(
                PlannerClient(TestFactory.PlantWithAccess), 
                9999, 
                // Forbidden due to lack of permission to project of unknown tag
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task Get_Tag_AsPreserver_ShouldReturnForbidden_WhenUnknownId()
            => await TagsControllerTestsHelper.GetTagAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                9999, 
                // Forbidden due to lack of permission to project of unknown tag
                HttpStatusCode.Forbidden);
        #endregion
    }
}
