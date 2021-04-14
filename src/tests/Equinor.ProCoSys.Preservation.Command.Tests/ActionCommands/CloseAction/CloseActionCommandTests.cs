using Equinor.ProCoSys.Preservation.Command.ActionCommands.CloseAction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ActionCommands.CloseAction
{
    [TestClass]
    public class CloseActionCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new CloseActionCommand(2, 1, "AAAAAAAAABA=");

            Assert.AreEqual(2, dut.TagId);
            Assert.AreEqual(1, dut.ActionId);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
