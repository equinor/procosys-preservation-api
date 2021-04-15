using Equinor.ProCoSys.Preservation.Command.ModeCommands.UpdateMode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ModeCommands.UpdateMode
{
    [TestClass]
    public class UpdateModeCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UpdateModeCommand(1, "ModeTitle", true, "AAAAAAAAABA=");
            Assert.AreEqual(1, dut.ModeId);
            Assert.AreEqual("ModeTitle", dut.Title);
            Assert.IsTrue(dut.ForSupplier);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
