using Equinor.Procosys.Preservation.Command.TagCommands.RecordCommands.RecordCheckBoxChecked;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.RecordCommands.RecordCheckBoxChecked
{
    [TestClass]
    public class RecordCheckBoxCheckedCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new RecordCheckBoxCheckedCommand(1, 2);

            Assert.AreEqual(1, dut.TagId);
            Assert.AreEqual(2, dut.FieldId);
        }
    }
}
