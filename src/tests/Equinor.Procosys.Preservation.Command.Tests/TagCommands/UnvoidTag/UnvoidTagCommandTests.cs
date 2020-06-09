using System;
using Equinor.Procosys.Preservation.Command.TagCommands.UnvoidTag;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.UnvoidTag
{
    [TestClass]
    public class UnvoidTagCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UnvoidTagCommand(2, "AAAAAAAAABA=", Guid.Empty);

            Assert.AreEqual(2, dut.TagId);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
            Assert.AreEqual(Guid.Empty, dut.CurrentUserOid);
        }
    }
}
