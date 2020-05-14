using Equinor.Procosys.Preservation.Command.ModeCommands.DeleteMode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.ModeCommands.DeleteMode
{
    [TestClass]
    public class DeleteModeCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new DeleteModeCommand(1, "AAAAAAAAABA=");

            Assert.AreEqual(1, dut.ModeId);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
