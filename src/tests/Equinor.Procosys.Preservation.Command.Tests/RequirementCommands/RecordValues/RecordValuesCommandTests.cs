using Equinor.Procosys.Preservation.Command.RequirementCommands.RecordValues;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementCommands.RecordValues
{
    [TestClass]
    public class RecordValuesCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new RecordValuesCommand(1, 2, null, null, "Comment");

            Assert.AreEqual(1, dut.TagId);
            Assert.AreEqual(2, dut.RequirementId);
            Assert.IsNotNull(dut.CheckBoxValues);
            Assert.IsNotNull(dut.NumberValues);
            Assert.AreEqual(0, dut.NumberValues.Count);
            Assert.AreEqual("Comment", dut.Comment);
        }
    }
}
