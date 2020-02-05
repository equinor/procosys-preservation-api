using System.Collections.Generic;
using Equinor.Procosys.Preservation.Command.TagCommands.RecordValues;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.RecordValues
{
    [TestClass]
    public class RecordCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            // todo more tests
            var dut = new RecordValuesCommand(1, 2, new List<FieldValue>(), default);

            Assert.AreEqual(1, dut.TagId);
            Assert.AreEqual(2, dut.RequirementDefinitionId);
        }
    }
}
