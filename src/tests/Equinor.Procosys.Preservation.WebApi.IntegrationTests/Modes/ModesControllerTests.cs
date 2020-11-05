﻿using System;
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
        public async Task CreateMode_AsAdmin_ShouldCreateMode()
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
        public async Task GetMode_AsAdmin_ShouldGetMode()
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
        public async Task UpdateMode_AsAdmin_ShouldUpdateModeAndRowVersion()
        {
            // Assert
            var id = await ModesControllerTestsHelper.CreateModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                Guid.NewGuid().ToString());
            var mode = await ModesControllerTestsHelper.GetModeAsync(LibraryAdminClient(TestFactory.PlantWithAccess), id);
            var currentRowVersion = mode.RowVersion;

            // Act
            var newRowVersion = await ModesControllerTestsHelper.UpdateModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                mode.Id,
                Guid.NewGuid().ToString(),
                currentRowVersion);

            // Assert
            Assert.AreNotEqual(currentRowVersion, newRowVersion);
        }

        [TestMethod]
        public async Task VoidMode_AsAdmin_ShouldVoidMode()
        {
            // Assert
            var id = await ModesControllerTestsHelper.CreateModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                Guid.NewGuid().ToString());
            var mode = await ModesControllerTestsHelper.GetModeAsync(LibraryAdminClient(TestFactory.PlantWithAccess), id);
            var currentRowVersion = mode.RowVersion;
            Assert.IsFalse(mode.IsVoided);

            // Act
            var newRowVersion = await ModesControllerTestsHelper.VoidModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                mode.Id,
                currentRowVersion);

            // Assert
            mode = await ModesControllerTestsHelper.GetModeAsync(LibraryAdminClient(TestFactory.PlantWithAccess), id);
            Assert.AreNotEqual(currentRowVersion, newRowVersion);
            Assert.IsTrue(mode.IsVoided);
        }

        [TestMethod]
        public async Task UnvoidMode_AsAdmin_ShouldUnvoidMode()
        {
            // Assert
            var id = await ModesControllerTestsHelper.CreateModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                Guid.NewGuid().ToString());
            var mode = await ModesControllerTestsHelper.GetModeAsync(LibraryAdminClient(TestFactory.PlantWithAccess), id);
            var currentRowVersion = await ModesControllerTestsHelper.VoidModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                mode.Id,
                mode.RowVersion);

            // Act
            var newRowVersion = await ModesControllerTestsHelper.UnvoidModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                mode.Id,
                currentRowVersion);

            // Assert
            mode = await ModesControllerTestsHelper.GetModeAsync(LibraryAdminClient(TestFactory.PlantWithAccess), id);
            Assert.AreNotEqual(currentRowVersion, newRowVersion);
            Assert.IsFalse(mode.IsVoided);
        }

        [TestMethod]
        public async Task DeleteMode_AsAdmin_ShouldDeleteMode()
        {
            // Assert
            var id = await ModesControllerTestsHelper.CreateModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                Guid.NewGuid().ToString());
            var mode = await ModesControllerTestsHelper.GetModeAsync(LibraryAdminClient(TestFactory.PlantWithAccess), id);
            var currentRowVersion = await ModesControllerTestsHelper.VoidModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                mode.Id,
                mode.RowVersion);

            // Act
            await ModesControllerTestsHelper.DeleteModeAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                mode.Id,
                currentRowVersion);

            // Assert
            var modes = await ModesControllerTestsHelper.GetAllModesAsync(LibraryAdminClient(TestFactory.PlantWithAccess));
            Assert.IsNull(modes.SingleOrDefault(m => m.Id == id));
        }
    }
}
