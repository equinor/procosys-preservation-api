using System.Net;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Tags
{
    [TestClass]
    public class TagsControllerNegativeTests : TagsControllerTestsBase
    {
        #region GetAll
        [TestMethod]
        public async Task GetAllTags_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                TestFactory.ProjectWithAccess,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetAllTags_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                TestFactory.ProjectWithAccess,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTags_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                TestFactory.ProjectWithAccess,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetAllTags_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllTags_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetAllTags_AsPlanner_ShouldReturnOk()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                PlannerClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess);

        [TestMethod]
        public async Task GetAllTags_AsPreserver_ShouldReturnOk()
            => await TagsControllerTestsHelper.GetAllTagsAsync(
                PreserverClient(TestFactory.PlantWithAccess),
                TestFactory.ProjectWithAccess);
        #endregion
        
        #region Get
        [TestMethod]
        public async Task GetTag_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.GetTagAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetTag_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetTagAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetTag_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.GetTagAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999, 
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetTag_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetTagAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                InitialTagId, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTag_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.GetTagAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                InitialTagId, 
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetTag_AsPlanner_ShouldReturnOK_WhenKnownId()
            => await TagsControllerTestsHelper.GetTagAsync(
                PlannerClient(TestFactory.PlantWithAccess), 
                InitialTagId);

        [TestMethod]
        public async Task GetTag_AsPreserver_ShouldReturnOK_WhenKnownId()
            => await TagsControllerTestsHelper.GetTagAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                InitialTagId);

        [TestMethod]
        public async Task GetTag_AsPlanner_ShouldReturnNotFound_WhenUnknownId()
            => await TagsControllerTestsHelper.GetTagAsync(
                PlannerClient(TestFactory.PlantWithAccess), 
                9999, 
                HttpStatusCode.NotFound);

        [TestMethod]
        public async Task GetTag_AsPreserver_ShouldReturnNotFound_WhenUnknownId()
            => await TagsControllerTestsHelper.GetTagAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                9999, 
                HttpStatusCode.NotFound);
        #endregion
        
        #region DuplicateAreaTag
        [TestMethod]
        public async Task DuplicateAreaTag_AsAnonymous_ShouldReturnUnauthorized()
            => await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                AnonymousClient(TestFactory.UnknownPlant),
                9999,
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                KnownAreaCode,
                null,
                "Desc",
                null,
                null,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DuplicateAreaTag_AsHacker_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                AuthenticatedHackerClient(TestFactory.UnknownPlant),
                9999,
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                KnownAreaCode,
                null,
                "Desc",
                null,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DuplicateAreaTag_AsAdmin_ShouldReturnBadRequest_WhenUnknownPlant()
            => await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                LibraryAdminClient(TestFactory.UnknownPlant),
                9999,
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                KnownAreaCode,
                null,
                "Desc",
                null,
                null,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DuplicateAreaTag_AsHacker_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                AuthenticatedHackerClient(TestFactory.PlantWithAccess),
                InitialTagId, 
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                KnownAreaCode,
                null,
                "Desc",
                null,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DuplicateAreaTag_AsAdmin_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess), 
                InitialTagId, 
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                KnownAreaCode,
                null,
                "Desc",
                null,
                null,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DuplicateAreaTag_AsPreserver_ShouldReturnForbidden_WhenPermissionMissing()
            => await TagsControllerTestsHelper.DuplicateAreaTagAsync(
                PreserverClient(TestFactory.PlantWithAccess), 
                InitialTagId, 
                AreaTagType.SiteArea,
                KnownDisciplineCode,
                KnownAreaCode,
                null,
                "Desc",
                null,
                null,
                HttpStatusCode.Forbidden);
        #endregion
    }
}
