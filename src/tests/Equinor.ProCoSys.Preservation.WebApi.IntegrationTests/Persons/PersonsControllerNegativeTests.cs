using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Persons
{
    [TestClass]
    public class PersonsControllerNegativeTests : TestBase
    {
        private int _filterIdUnderTest;

        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            var filter = await PersonsControllerTestsHelper.CreateAndGetFilterAsync(
                    UserType.Preserver,
                    TestFactory.PlantWithAccess,
                    TestFactory.ProjectWithAccess,
                    Guid.NewGuid().ToString());
            _filterIdUnderTest = filter.Id;
        }

        #region GetSavedFilters
        [TestMethod]
        public async Task GetSavedFilters_AsAnonymous_ShouldReturnUnauthorized()
            => await PersonsControllerTestsHelper.GetSavedFiltersInProjectAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                TestFactory.ProjectWithAccess,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task GetSavedFilters_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await PersonsControllerTestsHelper.GetSavedFiltersInProjectAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                "P",
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task GetSavedFilters_AsPreserver_ShouldReturnBadRequest_WhenUnknownPlant()
            => await PersonsControllerTestsHelper.GetSavedFiltersInProjectAsync(
                UserType.Preserver,
                TestFactory.UnknownPlant,
                "P",
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task GetSavedFilters_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await PersonsControllerTestsHelper.GetSavedFiltersInProjectAsync(
                UserType.Hacker,
                TestFactory.PlantWithoutAccess,
                "P",
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Create
        [TestMethod]
        public async Task CreateSavedFilter_AsAnonymous_ShouldReturnUnauthorized()
            => await PersonsControllerTestsHelper.CreateSavedFilterAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                "P",
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                false,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task CreateSavedFilter_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await PersonsControllerTestsHelper.CreateSavedFilterAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                "P",
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                false,
                HttpStatusCode.Forbidden,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateSavedFilter_AsPreserver_ShouldReturnBadRequest_WhenUnknownPlant()
            => await PersonsControllerTestsHelper.CreateSavedFilterAsync(
                UserType.Preserver,
                TestFactory.UnknownPlant,
                "P",
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                false,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task CreateSavedFilter_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await PersonsControllerTestsHelper.CreateSavedFilterAsync(
                UserType.Hacker,
                TestFactory.PlantWithoutAccess,
                "P",
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                false,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task CreateSavedFilter_AsPreserver_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await PersonsControllerTestsHelper.CreateSavedFilterAsync(
                UserType.Preserver,
                TestFactory.PlantWithoutAccess,
                "P",
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                false,
                HttpStatusCode.Forbidden);
        #endregion
        
        #region Update
        [TestMethod]
        public async Task UpdateSavedFilter_AsAnonymous_ShouldReturnUnauthorized()
            => await PersonsControllerTestsHelper.UpdateSavedFilterAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                _filterIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                false,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task UpdateSavedFilter_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await PersonsControllerTestsHelper.UpdateSavedFilterAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                _filterIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                false,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateSavedFilter_AsPreserver_ShouldReturnBadRequest_WhenUnknownPlant()
            => await PersonsControllerTestsHelper.UpdateSavedFilterAsync(
                UserType.Preserver,
                TestFactory.UnknownPlant,
                _filterIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                false,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task UpdateSavedFilter_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await PersonsControllerTestsHelper.UpdateSavedFilterAsync(
                UserType.Hacker,
                TestFactory.PlantWithoutAccess,
                _filterIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                false,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateSavedFilter_AsPreserver_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await PersonsControllerTestsHelper.UpdateSavedFilterAsync(
                UserType.Preserver,
                TestFactory.PlantWithoutAccess,
                _filterIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                false,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task UpdateSavedFilter_AsPreserver_ShouldReturnConflict_WhenWrongRowVersion()
            => await PersonsControllerTestsHelper.UpdateSavedFilterAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                _filterIdUnderTest,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                false,
                TestFactory.WrongButValidRowVersion,
                HttpStatusCode.Conflict);
        
        #endregion

        #region Delete
        [TestMethod]
        public async Task DeleteSavedFilter_AsAnonymous_ShouldReturnUnauthorized()
            => await PersonsControllerTestsHelper.DeleteSavedFilterAsync(
                UserType.Anonymous,
                TestFactory.UnknownPlant,
                _filterIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Unauthorized);

        [TestMethod]
        public async Task DeleteSavedFilter_AsHacker_ShouldReturnForbidden_WhenUnknownPlant()
            => await PersonsControllerTestsHelper.DeleteSavedFilterAsync(
                UserType.Hacker,
                TestFactory.UnknownPlant,
                _filterIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteSavedFilter_AsPreserver_ShouldReturnBadRequest_WhenUnknownPlant()
            => await PersonsControllerTestsHelper.DeleteSavedFilterAsync(
                UserType.Preserver,
                TestFactory.UnknownPlant,
                _filterIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.BadRequest,
                "is not a valid plant");

        [TestMethod]
        public async Task DeleteSavedFilter_AsHacker_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await PersonsControllerTestsHelper.DeleteSavedFilterAsync(
                UserType.Hacker,
                TestFactory.PlantWithoutAccess,
                _filterIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteSavedFilter_AsPreserver_ShouldReturnForbidden_WhenNoAccessToPlant()
            => await PersonsControllerTestsHelper.DeleteSavedFilterAsync(
                UserType.Preserver,
                TestFactory.PlantWithoutAccess,
                _filterIdUnderTest,
                TestFactory.AValidRowVersion,
                HttpStatusCode.Forbidden);

        [TestMethod]
        public async Task DeleteSavedFilter_AsPreserver_ShouldReturnConflict_WhenWrongRowVersion()
        {
            var filter = await PersonsControllerTestsHelper.CreateAndGetFilterAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess,
                Guid.NewGuid().ToString());

            // Act
            await PersonsControllerTestsHelper.DeleteSavedFilterAsync(
                           UserType.Preserver,
                           TestFactory.PlantWithAccess,
                           filter.Id,
                           TestFactory.WrongButValidRowVersion,
                           HttpStatusCode.Conflict);
        }
        #endregion
    }
}
