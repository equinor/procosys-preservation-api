using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.TagFunctions
{
    [TestClass]
    public class TagFunctionsControllerTests : TagFunctionsControllerTestsBase
    {
        [TestMethod]
        public async Task UpdateTagFunction_AsAdmin_ShouldCreateTagFunctionWithRequirements()
        {
            // Arrange
            var newReqDefId = await CreateRequirementDefinitionAsync(TestFactory.PlantWithAccess);
            var intervalWeeks = 4;

            // Act
            var tagFunction = await UpdateAndGetTagFunctionAsync(TestFactory.PlantWithAccess,
                TagFunctionUnderTest.Code,
                TagFunctionUnderTest.RegisterCode,
                newReqDefId,
                intervalWeeks);

            // Assert
            Assert.IsNotNull(tagFunction);
            Assert.AreEqual(TagFunctionUnderTest.Code, tagFunction.Code);
            Assert.AreEqual(TagFunctionUnderTest.RegisterCode, tagFunction.RegisterCode);
            Assert.AreEqual(1, tagFunction.Requirements.Count());
            Assert.AreEqual(newReqDefId, tagFunction.Requirements.Single().RequirementDefinitionId);
            Assert.AreEqual(intervalWeeks, tagFunction.Requirements.Single().IntervalWeeks);
        }

        [TestMethod]
        public async Task VoidTagFunction_AsAdmin_ShouldVoidTagFunction()
        {
            // Arrange
            var currentRowVersion = await EnsureTagFunctionIsUnvoidedAsync(
                TestFactory.PlantWithAccess,
                TagFunctionUnderVoidingTest.Code,
                TagFunctionUnderVoidingTest.RegisterCode);

            // Act
            var newRowVersion = await TagFunctionsControllerTestsHelper.VoidTagFunctionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                TagFunctionUnderVoidingTest.Code,
                TagFunctionUnderVoidingTest.RegisterCode,
                currentRowVersion);

            // Assert
            var tagFunction = await TagFunctionsControllerTestsHelper.GetTagFunctionDetailsAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                TagFunctionUnderVoidingTest.Code,
                TagFunctionUnderVoidingTest.RegisterCode);

            AssertRowVersionChange(currentRowVersion, newRowVersion);
            Assert.IsTrue(tagFunction.IsVoided);
        }

        [TestMethod]
        public async Task UnvoidTagFunction_AsAdmin_ShouldUnvoidTagFunction()
        {
            // Arrange
            var currentRowVersion = await EnsureTagFunctionIsVoidedAsync(
                TestFactory.PlantWithAccess,
                TagFunctionUnderUnvoidingTest.Code,
                TagFunctionUnderUnvoidingTest.RegisterCode);

            // Act
            var newRowVersion = await TagFunctionsControllerTestsHelper.UnvoidTagFunctionAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                TagFunctionUnderUnvoidingTest.Code,
                TagFunctionUnderUnvoidingTest.RegisterCode,
                currentRowVersion);

            // Assert
            var tagFunction = await TagFunctionsControllerTestsHelper.GetTagFunctionDetailsAsync(
                UserType.LibraryAdmin,
                TestFactory.PlantWithAccess,
                TagFunctionUnderUnvoidingTest.Code,
                TagFunctionUnderUnvoidingTest.RegisterCode);
            AssertRowVersionChange(currentRowVersion, newRowVersion);
            Assert.IsFalse(tagFunction.IsVoided);
        }
    }
}
