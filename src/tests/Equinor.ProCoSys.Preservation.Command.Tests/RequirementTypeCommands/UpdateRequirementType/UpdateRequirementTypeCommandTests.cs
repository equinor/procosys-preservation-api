using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UpdateRequirementType;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementTypeCommands.UpdateRequirementType
{
    [TestClass]
    public class UpdateRequirementTypeCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UpdateRequirementTypeCommand(1, "AAAAAAAAABA=", 3, "TitleA", "CodeA", RequirementTypeIcon.Battery);

            Assert.AreEqual(1, dut.RequirementTypeId);
            Assert.AreEqual("TitleA", dut.Title);
            Assert.AreEqual("CodeA", dut.Code);
            Assert.AreEqual(3, dut.SortKey);
            Assert.AreEqual(RequirementTypeIcon.Battery, dut.Icon);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
