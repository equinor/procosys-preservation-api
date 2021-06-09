using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Modes
{
    [TestClass]
    public class ModesControllerTests : TestBase
    {
        private int _initialModesCount;
        private int _modeIdUnderTest;

        [TestInitialize]
        public async Task TestInitialize()
        {
            var modes = await ModesControllerTestsHelper.GetAllModesAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);

            _initialModesCount = modes.Count;

            _modeIdUnderTest = TestFactory.Instance.SeededData[KnownPlantData.PlantA].OtherModeId;
        }

        [TestMethod]
        public async Task CreateMode_AsAdmin_ShouldCreateMode()
        {
            // Act
            var title = Guid.NewGuid().ToString();
            var id = await ModesControllerTestsHelper.CreateModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                title);

            // Assert
            Assert.IsTrue(id > 0);
            var modes = await ModesControllerTestsHelper.GetAllModesAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            Assert.AreEqual(_initialModesCount+1, modes.Count);
            var mode = modes.SingleOrDefault(m => m.Id == id);
            Assert.IsNotNull(mode);
            Assert.AreEqual(title, mode.Title);
        }

        [TestMethod]
        public async Task GetMode_AsAdmin_ShouldGetMode()
        {
            // Act
            var mode = await ModesControllerTestsHelper.GetModeAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess, _modeIdUnderTest);

            // Assert
            Assert.AreEqual(_modeIdUnderTest, mode.Id);
            Assert.IsNotNull(mode.RowVersion);
        }

        [TestMethod]
        public async Task UpdateMode_AsAdmin_ShouldUpdateModeAndRowVersion()
        {
            var mode = await ModesControllerTestsHelper.GetModeAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess, _modeIdUnderTest);
            var currentRowVersion = mode.RowVersion;

            // Act
            var newRowVersion = await ModesControllerTestsHelper.UpdateModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                mode.Id,
                Guid.NewGuid().ToString(),
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
        }

        [TestMethod]
        public async Task VoidMode_AsAdmin_ShouldVoidMode()
        {
            // Arrange
            var id = await ModesControllerTestsHelper.CreateModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());
            var mode = await ModesControllerTestsHelper.GetModeAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess, id);
            var currentRowVersion = mode.RowVersion;
            Assert.IsFalse(mode.IsVoided);

            // Act
            var newRowVersion = await ModesControllerTestsHelper.VoidModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                mode.Id,
                currentRowVersion);

            // Assert
            mode = await ModesControllerTestsHelper.GetModeAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess, id);
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            Assert.IsTrue(mode.IsVoided);
        }

        [TestMethod]
        public async Task UnvoidMode_AsAdmin_ShouldUnvoidMode()
        {
            // Arrange
            var id = await ModesControllerTestsHelper.CreateModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());
            var mode = await ModesControllerTestsHelper.GetModeAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess, id);
            var currentRowVersion = await ModesControllerTestsHelper.VoidModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                mode.Id,
                mode.RowVersion);

            // Act
            var newRowVersion = await ModesControllerTestsHelper.UnvoidModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                mode.Id,
                currentRowVersion);

            // Assert
            mode = await ModesControllerTestsHelper.GetModeAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess, id);
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            Assert.IsFalse(mode.IsVoided);
        }

        [TestMethod]
        public async Task DeleteMode_AsAdmin_ShouldDeleteMode()
        {
            // Arrange
            var id = await ModesControllerTestsHelper.CreateModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());
            var mode = await ModesControllerTestsHelper.GetModeAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess, id);
            var currentRowVersion = await ModesControllerTestsHelper.VoidModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                mode.Id,
                mode.RowVersion);

            // Act
            await ModesControllerTestsHelper.DeleteModeAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                mode.Id,
                currentRowVersion);

            // Assert
            var modes = await ModesControllerTestsHelper.GetAllModesAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            Assert.IsNull(modes.SingleOrDefault(m => m.Id == id));
        }
    }
}
