using System;
using Equinor.Procosys.Preservation.Command.ModeCommands.UpdateMode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.ModeCommands.UpdateMode
{
    [TestClass]
    public class UpdateModeCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UpdateModeCommand(1, "ModeTitle", true, "AAAAAAAAABA=", Guid.Empty);
            Assert.AreEqual(1, dut.ModeId);
            Assert.AreEqual("ModeTitle", dut.Title);
            Assert.IsTrue(dut.ForSupplier);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
            Assert.AreEqual(Guid.Empty, dut.CurrentUserOid);
        }
    }
}
