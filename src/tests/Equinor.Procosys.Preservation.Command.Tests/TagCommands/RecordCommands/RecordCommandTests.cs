using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.RecordCommands
{
    [TestClass]
    public class RecordCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new TestCommand(1, 2);

            Assert.AreEqual(1, dut.TagId);
            Assert.AreEqual(2, dut.FieldId);
        }
    }
}
