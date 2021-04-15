using Equinor.ProCoSys.Preservation.Command.TagCommands.Preserve;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.Preserve
{
    [TestClass]
    public class PreserveCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new PreserveCommand(17);

            Assert.AreEqual(17, dut.TagId);
        }
    }
}
