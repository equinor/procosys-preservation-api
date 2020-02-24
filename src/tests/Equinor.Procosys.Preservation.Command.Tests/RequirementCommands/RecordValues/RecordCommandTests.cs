using Equinor.Procosys.Preservation.Command.RequirementCommands.RecordValues;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementCommands.RecordValues
{
    [TestClass]
    public class RecordCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new RecordValuesCommand(1, 2, null, "Comment");

            Assert.AreEqual(1, dut.TagId);
            Assert.AreEqual(2, dut.RequirementId);
            Assert.IsNotNull(dut.FieldValues);
            Assert.AreEqual(0, dut.FieldValues.Count);
            Assert.AreEqual("Comment", dut.Comment);
        }
    }
}
