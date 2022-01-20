using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Persons
{
    [TestClass]
    public class PersonsControllerTests : TestBase
    {
        [TestMethod]
        public async Task CreateSavedFilter_AsPreserver_ShouldCreateSavedFilter()
        {
            var title = Guid.NewGuid().ToString();

            // Act
            var filter = await PersonsControllerTestsHelper.CreateAndGetFilterAsync(
                UserType.Preserver, 
                TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess,
                title);

            // Assert
            Assert.AreEqual(title, filter.Title);
        }

        [TestMethod]
        public async Task UpdateSavedFilter_AsPreserver_ShouldUpdateSavedFilterAndRowVersion()
        {
            var filter = await PersonsControllerTestsHelper.CreateAndGetFilterAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess,
                Guid.NewGuid().ToString());
            var newTitle = Guid.NewGuid().ToString();

            // Act
            var newRowVersion = await PersonsControllerTestsHelper.UpdateSavedFilterAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                filter.Id,
                newTitle,
                Guid.NewGuid().ToString(), 
                false,
                filter.RowVersion);

            // Assert
            AssertRowVersionChange(filter.RowVersion, newRowVersion);
            var filters = await PersonsControllerTestsHelper.GetSavedFiltersInProjectAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);
            filter = filters.Single(f => f.Id == filter.Id);
            Assert.AreEqual(newTitle, filter.Title);
        }

        [TestMethod]
        public async Task DeleteSavedFilter_AsPreserver_ShouldDeleteSavedFilter()
        {
            // Arrange
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
                filter.RowVersion);

            // Assert
            var filters = await PersonsControllerTestsHelper.GetSavedFiltersInProjectAsync(
                UserType.Preserver,
                TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess);
            Assert.IsNull(filters.SingleOrDefault(m => m.Id == filter.Id));
        }
    }
}
