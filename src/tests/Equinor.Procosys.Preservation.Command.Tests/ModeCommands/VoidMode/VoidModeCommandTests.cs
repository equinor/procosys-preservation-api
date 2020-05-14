using Equinor.Procosys.Preservation.Command.ModeCommands.VoidMode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.ModeCommands.VoidMode
{
    [TestClass]
    public class VoidModeCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new VoidModeCommand(1, "AAAAAAAAABA=");

            Assert.AreEqual(1, dut.ModeId);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
