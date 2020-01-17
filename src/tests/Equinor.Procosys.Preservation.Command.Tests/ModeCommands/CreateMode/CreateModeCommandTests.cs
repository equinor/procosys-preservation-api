using Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.ModeCommands.CreateMode
{
    [TestClass]
    public class CreateModeCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new CreateModeCommand("TitleA");

            Assert.AreEqual("TitleA", dut.Title);
        }
    }
}
