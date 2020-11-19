using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.RequirementTypes
{
    [TestClass]
    public class RequirementTypesControllerTests : RequirementTypesControllerTestsBase
    {
        [TestMethod]
        public async Task GetRequirementTypes_AsAdmin_ShouldGetRequirementTypesWithReqDefs()
        {
            // Act
            var reqTypes = await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess));

            // Assert
            Assert.IsNotNull(reqTypes);
            Assert.AreNotEqual(0, reqTypes.Count);
            var reqDef = reqTypes.First().RequirementDefinitions.FirstOrDefault();
            Assert.IsNotNull(reqDef);
        }

        [TestMethod]
        public async Task CreateRequirementDefinition_AsAdmin_ShouldCreateRequirementDefinition()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var title = Guid.NewGuid().ToString();

            // Act
            var reqDefId = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                title);

            // Assert
            var reqDef = await GetRequirementDefinitionDetailsAsync(reqTypeIdUnderTest, reqDefId);
            Assert.IsNotNull(reqDef);
        }

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldUpdateRequirementDefinitionAndRowVersion()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDefIdUnderTest = ReqDefInReqTypeAIdUnderTest;
            var reqDef = await GetRequirementDefinitionDetailsAsync(reqTypeIdUnderTest, reqDefIdUnderTest);
            var currentRowVersion = reqDef.RowVersion;
            var newTitle = Guid.NewGuid().ToString();

            // Act
            var newRowVersion = await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                reqDef.Id,
                newTitle,
                4,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            reqDef = await GetRequirementDefinitionDetailsAsync(reqTypeIdUnderTest, reqDefIdUnderTest);
            Assert.AreEqual(newTitle, reqDef.Title);
        }

        [TestMethod]
        public async Task VoidRequirementDefinition_AsAdmin_ShouldVoidRequirementDefinition_AndUpdateAndRowVersion()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDefId = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                Guid.NewGuid().ToString());
            var reqDef = await GetRequirementDefinitionDetailsAsync(reqTypeIdUnderTest, reqDefId);
            var currentRowVersion = reqDef.RowVersion;
            Assert.IsFalse(reqDef.IsVoided);

            // Act
            var newRowVersion = await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                reqDefId,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            reqDef = await GetRequirementDefinitionDetailsAsync(reqTypeIdUnderTest, reqDefId);
            Assert.IsTrue(reqDef.IsVoided);
        }

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAdmin_ShouldUnvoidRequirementDefinition_AndUpdateAndRowVersion()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDefId = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                Guid.NewGuid().ToString());
            var reqDef = await GetRequirementDefinitionDetailsAsync(reqTypeIdUnderTest, reqDefId);
            var currentRowVersion = await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                reqDefId,
                reqDef.RowVersion);

            // Act
            var newRowVersion = await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                reqDefId,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            reqDef = await GetRequirementDefinitionDetailsAsync(reqTypeIdUnderTest, reqDefId);
            Assert.IsFalse(reqDef.IsVoided);
        }

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAdmin_ShouldDeleteRequirementDefinition()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDefId = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                Guid.NewGuid().ToString());
            var reqDef = await GetRequirementDefinitionDetailsAsync(reqTypeIdUnderTest, reqDefId);
            var currentRowVersion = await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                reqDefId,
                reqDef.RowVersion);

            // Act
            await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                reqDefId,
                currentRowVersion);

            // Assert
            reqDef = await GetRequirementDefinitionDetailsAsync(reqTypeIdUnderTest, reqDefId);
            Assert.IsNull(reqDef);
        }

        private async Task<RequirementDefinitionDto> GetRequirementDefinitionDetailsAsync(int reqTypeId, int reqDefId)
        {
            var reqType = await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess));
            return reqType
                .Single(r => r.Id == reqTypeId)
                .RequirementDefinitions
                .SingleOrDefault(s => s.Id == reqDefId);
        }
    }
}
