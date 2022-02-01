using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.RequirementTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests
{
    [TestClass]
    public abstract class TestBase
    {
        private readonly RowVersionValidator _rowVersionValidator = new RowVersionValidator();
        
        [AssemblyCleanup]
        public static void AssemblyCleanup() => TestFactory.Instance.Dispose();

        public void AssertRowVersionChange(string oldRowVersion, string newRowVersion)
        {
            Assert.IsTrue(_rowVersionValidator.IsValid(oldRowVersion));
            Assert.IsTrue(_rowVersionValidator.IsValid(newRowVersion));
            Assert.AreNotEqual(oldRowVersion, newRowVersion);
        }

        protected async Task<int> CreateRequirementDefinitionAsync(string plant)
        {
            var reqTypes = await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(UserType.LibraryAdmin, plant);
            var newReqDefId = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.LibraryAdmin, plant, reqTypes.First().Id, Guid.NewGuid().ToString());
            return newReqDefId;
        }
    }
}
