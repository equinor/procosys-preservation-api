using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.RequirementTypes
{
    [TestClass]
    public class RequirementTypesControllerTests : RequirementTypesControllerTestsBase
    {
        [TestMethod]
        public async Task CreateRequirementType_AsAdmin_ShouldCreateRequirementType()
        {
            // Arrange
            var title = Guid.NewGuid().ToString();

            // Act
            var reqType = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                title);

            // Assert
            Assert.AreEqual(title, reqType.Title);
        }

        [TestMethod]
        public async Task UpdateRequirementType_AsAdmin_ShouldUpdateRequirementType()
        {
            // Arrange
            var reqType = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());
            var currentRowVersion = reqType.RowVersion;
            var newTitle = Guid.NewGuid().ToString();

            // Act
            var newRowVersion = await RequirementTypesControllerTestsHelper.UpdateRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id,
                newTitle,
                reqType.RowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            reqType = await RequirementTypesControllerTestsHelper.GetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id);
            Assert.AreEqual(newTitle, reqType.Title);
        }

        [TestMethod]
        public async Task GetRequirementTypes_AsAdmin_ShouldGetRequirementTypesWithReqDefs()
        {
            // Act
            var reqTypes = await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess);

            // Assert
            Assert.IsNotNull(reqTypes);
            Assert.AreNotEqual(0, reqTypes.Count);
            var reqDef = reqTypes.First().RequirementDefinitions.FirstOrDefault();
            Assert.IsNotNull(reqDef);
        }

        [TestMethod]
        public async Task GetRequirementType_AsAdmin_ShouldGetRequirementTypeWithReqDefs()
        {
            // Act
            var reqType = await RequirementTypesControllerTestsHelper.GetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                ReqTypeAIdUnderTest);

            // Assert
            Assert.IsNotNull(reqType);
            var reqDef = reqType.RequirementDefinitions.FirstOrDefault();
            Assert.IsNotNull(reqDef);
        }

        [TestMethod]
        public async Task VoidRequirementType_AsAdmin_ShouldVoidRequirementType()
        {
            // Arrange
            var reqType = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());
            var currentRowVersion = reqType.RowVersion;

            // Act
            var newRowVersion = await RequirementTypesControllerTestsHelper.VoidRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            reqType = await RequirementTypesControllerTestsHelper.GetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id);
            Assert.IsTrue(reqType.IsVoided);
        }

        [TestMethod]
        public async Task UnvoidRequirementType_AsAdmin_ShouldUnvoidRequirementType()
        {
            // Arrange
            var reqType = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());

            var currentRowVersion = await RequirementTypesControllerTestsHelper.VoidRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id,
                reqType.RowVersion);

            // Act
            var newRowVersion = await RequirementTypesControllerTestsHelper.UnvoidRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            reqType = await RequirementTypesControllerTestsHelper.GetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id);
            Assert.IsFalse(reqType.IsVoided);
        }

        [TestMethod]
        public async Task DeleteRequirementType_AsAdmin_ShouldDeleteRequirementType()
        {
            // Arrange
            var reqType = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                Guid.NewGuid().ToString());

            var currentRowVersion = await RequirementTypesControllerTestsHelper.VoidRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id,
                reqType.RowVersion);

            // Act
            await RequirementTypesControllerTestsHelper.DeleteRequirementTypeAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqType.Id,
                currentRowVersion);

            // Assert
            var reqTypes = await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess);
            Assert.IsNull(reqTypes.SingleOrDefault(m => m.Id == reqType.Id));
        }

        [TestMethod]
        public async Task CreateRequirementDefinition_AsAdmin_ShouldCreateRequirementDefinition()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var title = Guid.NewGuid().ToString();

            // Act
            var reqDefId = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                title);

            // Assert
            var reqDef =
                await RequirementTypesControllerTestsHelper.GetRequirementDefinitionDetailsAsync(
                    UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                    reqTypeIdUnderTest,
                    reqDefId);
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
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
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
            var reqDef = await RequirementTypesControllerTestsHelper.GetRequirementDefinitionDetailsAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDefId);
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
            var reqDef = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest);
            var currentRowVersion = reqDef.RowVersion;
            var newTitle = Guid.NewGuid().ToString();

            // Act
            var newRowVersion = await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                newTitle,
                4,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            reqDef = await RequirementTypesControllerTestsHelper.GetRequirementDefinitionDetailsAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id);
            Assert.AreEqual(newTitle, reqDef.Title);
        }

        [TestMethod]
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldUpdateFieldAndFieldRowVersion()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDef = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                new List<FieldDto>
                {
                    new FieldDto
                    {
                        FieldType = FieldType.Info,
                        Label = Guid.NewGuid().ToString(),
                        SortKey = 20
                    }
                });

            var fieldDetailsDto = reqDef.Fields.Single();
            var oldFieldRowVersion = fieldDetailsDto.RowVersion;
            var newFieldLabel = Guid.NewGuid().ToString();
            fieldDetailsDto.Label = newFieldLabel;

            // Act
            await RequirementTypesControllerTestsHelper.UpdateRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                reqDef.Title,
                4,
                reqDef.RowVersion,
                reqDef.Fields.ToList());

            // Assert
            reqDef = await RequirementTypesControllerTestsHelper.GetRequirementDefinitionDetailsAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id);
            fieldDetailsDto = reqDef.Fields.Single();
            AssertRowVersionChange(oldFieldRowVersion, fieldDetailsDto.RowVersion);
            Assert.AreEqual(newFieldLabel, fieldDetailsDto.Label);
        }

        [TestMethod]
        public async Task VoidRequirementDefinition_AsAdmin_ShouldVoidRequirementDefinition_AndUpdateAndRowVersion()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDef = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest);
            var currentRowVersion = reqDef.RowVersion;
            Assert.IsFalse(reqDef.IsVoided);

            // Act
            var newRowVersion = await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            reqDef = await RequirementTypesControllerTestsHelper.GetRequirementDefinitionDetailsAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id);
            Assert.IsTrue(reqDef.IsVoided);
        }

        [TestMethod]
        public async Task UnvoidRequirementDefinition_AsAdmin_ShouldUnvoidRequirementDefinition_AndUpdateAndRowVersion()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDef = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest);
            var currentRowVersion = await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                reqDef.RowVersion);

            // Act
            var newRowVersion = await RequirementTypesControllerTestsHelper.UnvoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                currentRowVersion);

            // Assert
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            reqDef = await RequirementTypesControllerTestsHelper.GetRequirementDefinitionDetailsAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id);
            Assert.IsFalse(reqDef.IsVoided);
        }

        [TestMethod]
        public async Task DeleteRequirementDefinition_AsAdmin_ShouldDeleteRequirementDefinition()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDef = await RequirementTypesControllerTestsHelper.CreateAndGetRequirementDefinitionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                reqTypeIdUnderTest);
            var currentRowVersion = await RequirementTypesControllerTestsHelper.VoidRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                reqDef.RowVersion);

            // Act
            await RequirementTypesControllerTestsHelper.DeleteRequirementDefinitionAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id,
                currentRowVersion);

            // Assert
            reqDef = await RequirementTypesControllerTestsHelper.GetRequirementDefinitionDetailsAsync(
                UserType.LibraryAdmin, TestFactory.PlantWithAccess,
                reqTypeIdUnderTest,
                reqDef.Id);
            Assert.IsNull(reqDef);
        }
    }
}
