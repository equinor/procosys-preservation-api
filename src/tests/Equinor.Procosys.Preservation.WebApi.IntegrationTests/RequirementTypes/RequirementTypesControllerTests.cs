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
        public async Task UpdateRequirementDefinition_AsAdmin_ShouldUpdateRequirementDefinitionAndRowVersion()
        {
            // Arrange
            var reqTypeIdUnderTest = ReqTypeAIdUnderTest;
            var reqDefIdUnderTest = ReqDefInReqTypeAIdUnderTest;
            var reqDef = await GetReqDefDetails(reqTypeIdUnderTest, reqDefIdUnderTest);
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
            reqDef = await GetReqDefDetails(reqTypeIdUnderTest, reqDefIdUnderTest);
            Assert.AreEqual(newTitle, reqDef.Title);
        }
        private async Task<RequirementDefinitionDto> GetReqDefDetails(int reqTypeId, int reqDefId)
        {
            var reqType = await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(
                LibraryAdminClient(TestFactory.PlantWithAccess));
            return reqType
                .Single(r => r.Id == reqTypeId)
                .RequirementDefinitions
                .Single(s => s.Id == reqDefId);
        }
    }
}
