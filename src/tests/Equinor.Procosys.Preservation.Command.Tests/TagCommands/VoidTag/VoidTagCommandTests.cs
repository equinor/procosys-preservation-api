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
            var dut = new VoidTagCommand(2);

            Assert.AreEqual(2, dut.TagId);
        }
    }
}
