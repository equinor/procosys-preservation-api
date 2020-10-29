using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Modes
{
    [TestClass]
    public class ModesControllerTests : TestBase
    {
        private int initialModesCount;

        [TestInitialize]
        public async Task TestInitialize()
        {
            var modes = await ModesControllerTestsHelper.GetAllModesAsync(LibraryAdminClient(TestFactory.PlantWithAccess));

            initialModesCount = modes.Count;
        }

        [TestMethod]
        public async Task Create_Mode_AsAdmin_ShouldCreateMode()
        {
            // Act
            var id = await ModesControllerTestsHelper.CreateModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                Guid.NewGuid().ToString());

            // Assert
            Assert.IsTrue(id > 0);
            var modes = await ModesControllerTestsHelper.GetAllModesAsync(LibraryAdminClient(TestFactory.PlantWithAccess));
            Assert.AreEqual(initialModesCount+1, modes.Count);
            Assert.IsNotNull(modes.SingleOrDefault(m => m.Id == id));
        }

        [TestMethod]
        public async Task Get_Mode_AsAdmin_ShouldGetMode()
        {
            // Arrange
            var title = Guid.NewGuid().ToString();
            var id = await ModesControllerTestsHelper.CreateModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                title);

            // Act
            var mode = await ModesControllerTestsHelper.GetModeAsync(LibraryAdminClient(TestFactory.PlantWithAccess), id);

            // Assert
            Assert.AreEqual(id, mode.Id);
            Assert.AreEqual(title, mode.Title);
            Assert.IsNotNull(mode.RowVersion);
        }

        [TestMethod]
        public async Task Update_Mode_AsAdmin_ShouldUpdateModeAndRowVersion()
        {
            // Assert
            var id = await ModesControllerTestsHelper.CreateModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                Guid.NewGuid().ToString());
            var mode = await ModesControllerTestsHelper.GetModeAsync(LibraryAdminClient(TestFactory.PlantWithAccess), id);

            // Act
            var newRowVersion = await ModesControllerTestsHelper.UpdateModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                mode.Id,
                Guid.NewGuid().ToString(),
                mode.RowVersion);

            // Assert
            Assert.AreNotEqual(mode.RowVersion, newRowVersion);
        }
    }
}
