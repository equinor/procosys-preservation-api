using Equinor.Procosys.Preservation.Query.TagFunctionAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.TagFunctionAggregate
{
    [TestClass]
    public class RequirementDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new RequirementDto(2, 3);

            Assert.AreEqual(2, dut.Id);
            Assert.AreEqual(3, dut.RequirementDefinitionId);
        }
    }
}
