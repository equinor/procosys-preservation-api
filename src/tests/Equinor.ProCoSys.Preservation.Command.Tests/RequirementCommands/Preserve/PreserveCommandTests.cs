using Equinor.ProCoSys.Preservation.Command.RequirementCommands.Preserve;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementCommands.Preserve
{
    [TestClass]
    public class PreserveCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new PreserveCommand(17, 11);

            Assert.AreEqual(17, dut.TagId);
            Assert.AreEqual(11, dut.RequirementId);
        }
    }
}
