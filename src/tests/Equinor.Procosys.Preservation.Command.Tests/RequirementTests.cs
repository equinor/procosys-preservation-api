using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests
{
    [TestClass]
    public class RequirementTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Requirement(6, 8);

            Assert.AreEqual(6, dut.RequirementDefinitionId);
            Assert.AreEqual(8, dut.IntervalWeeks);
        }
    }
}
