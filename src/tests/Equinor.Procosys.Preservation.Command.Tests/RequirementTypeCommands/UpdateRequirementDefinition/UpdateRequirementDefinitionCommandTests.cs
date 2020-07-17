using System.Collections.Generic;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementType;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.UpdateRequirementDefinition
{
    [TestClass]
    public class UpdateRequirementDefinitionCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UpdateRequirementDefinitionCommand(1, 
                2, 
                10, 
                RequirementUsage.ForAll, 
                "Title", 
                4, 
                "AAAAAAAAABA=", 
                new List<UpdateFieldsForCommand>(), 
                new List<FieldsForCommand>());

            Assert.AreEqual(1, dut.RequirementTypeId);
            Assert.AreEqual(2, dut.RequirementDefinitionId);
            Assert.AreEqual(RequirementUsage.ForAll, dut.Usage);
            Assert.AreEqual("Title", dut.Title);
            Assert.AreEqual(4, dut.DefaultIntervalWeeks);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
            Assert.AreEqual(new List<UpdateFieldsForCommand>().Count, dut.UpdateFields.Count);
            Assert.AreEqual(new List<FieldsForCommand>().Count, dut.NewFields.Count);
        }
    }
}
