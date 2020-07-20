using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementDefinition;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementType;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.CreateRequirementDefinition
{
    [TestClass]
    public class CreateRequirementDefinitionCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new CreateRequirementDefinitionCommand(1, 10, RequirementUsage.ForAll, "title", 4);

            Assert.AreEqual("title", dut.Title);
            Assert.AreEqual(null, dut.Fields);
            Assert.AreEqual(1, dut.RequirementTypeId);
            Assert.AreEqual(10, dut.SortKey);
            Assert.AreEqual(4, dut.DefaultIntervalWeeks);
        }
    }
}
