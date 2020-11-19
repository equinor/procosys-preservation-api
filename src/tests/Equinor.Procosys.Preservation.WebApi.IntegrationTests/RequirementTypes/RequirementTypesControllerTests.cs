using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
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
            Assert.AreEqual(title, reqDef.Title);
            Assert.IsNotNull(reqDef.Fields);
            Assert.AreEqual(0, reqDef.Fields.Count());
        }

        [TestMethod]
        public async Task CreateRequirementDefinition_AsAdmin_ShouldCreateRequirementDefinitionWithField()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var title = Guid.NewGuid().ToString();
            var label = Guid.NewGuid().ToString();

            // Act
            var reqDefId = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                title,
                new List<FieldDto>
                {
                    new FieldDto
                    {
                        FieldType = FieldType.Info,
                        Label = label,
                        SortKey = 20
                    }
                });

            // Assert
            var reqDef = await GetRequirementDefinitionDetailsAsync(reqTypeIdUnderTest, reqDefId);
            Assert.IsNotNull(reqDef);
            Assert.AreEqual(title, reqDef.Title);
            Assert.IsNotNull(reqDef.Fields);
            Assert.AreEqual(1, reqDef.Fields.Count());
            Assert.AreEqual(label, reqDef.Fields.Single().Label);
        }

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldUpdateRequirementDefinitionAndRowVersion()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDefIdUnderTest = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                Guid.NewGuid().ToString());
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
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldUpdateFieldAndFieldRowVersion()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDefIdUnderTest = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                Guid.NewGuid().ToString(),
                new List<FieldDto>
                {
                    new FieldDto
                    {
                        FieldType = FieldType.Info,
                        Label = Guid.NewGuid().ToString(),
                        SortKey = 20
                    }
                });
            var reqDef = await GetRequirementDefinitionDetailsAsync(reqTypeIdUnderTest, reqDefIdUnderTest);
            
            var fieldDetailsDto = reqDef.Fields.Single();
            var oldFieldRowVersion = fieldDetailsDto.RowVersion;
            var newFieldLabel = Guid.NewGuid().ToString();
            fieldDetailsDto.Label = newFieldLabel;

            // Act
            await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess),
                reqTypeIdUnderTest,
                reqDef.Id,
                reqDef.Title,
                4,
                reqDef.RowVersion,
                reqDef.Fields.ToList());

            // Assert
            reqDef = await GetRequirementDefinitionDetailsAsync(reqTypeIdUnderTest, reqDefIdUnderTest);
            fieldDetailsDto = reqDef.Fields.Single();
            AssertRowVersionChange(oldFieldRowVersion, fieldDetailsDto.RowVersion);
            Assert.AreEqual(newFieldLabel, fieldDetailsDto.Label);
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
