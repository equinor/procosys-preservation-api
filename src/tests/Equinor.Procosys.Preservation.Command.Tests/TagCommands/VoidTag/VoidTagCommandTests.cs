using Equinor.ProCoSys.Preservation.Command.TagCommands.VoidTag;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.VoidTag
{
    [TestClass]
    public class VoidTagCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new VoidTagCommand(2, "AAAAAAAAABA=");

            Assert.AreEqual(2, dut.TagId);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
