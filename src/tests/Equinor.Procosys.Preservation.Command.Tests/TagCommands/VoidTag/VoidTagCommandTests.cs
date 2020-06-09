using System;
using Equinor.Procosys.Preservation.Command.TagCommands.VoidTag;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.VoidTag
{
    [TestClass]
    public class VoidTagCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new VoidTagCommand(2, "AAAAAAAAABA=", Guid.Empty);

            Assert.AreEqual(2, dut.TagId);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
            Assert.AreEqual(Guid.Empty, dut.CurrentUserOid);
        }
    }
}
