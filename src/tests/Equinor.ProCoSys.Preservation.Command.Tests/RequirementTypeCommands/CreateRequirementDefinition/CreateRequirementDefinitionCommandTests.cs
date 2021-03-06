﻿using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.CreateRequirementDefinition;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementTypeCommands.CreateRequirementDefinition
{
    [TestClass]
    public class CreateRequirementDefinitionCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new CreateRequirementDefinitionCommand(1, 10, RequirementUsage.ForAll, "title", 4);

            Assert.AreEqual("title", dut.Title);
            Assert.AreEqual(0, dut.Fields.Count);
            Assert.AreEqual(1, dut.RequirementTypeId);
            Assert.AreEqual(10, dut.SortKey);
            Assert.AreEqual(4, dut.DefaultIntervalWeeks);
        }
    }
}
