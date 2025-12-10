using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.TagCommands.CreateAreaTag;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.CreateAreaTag
{
    [TestClass]
    public class CreateAreaTagCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties_WithRequirements()
        {
            var dut = new CreateAreaTagCommand(
                "ProjectNameA",
                TagType.PreArea,
                "DisciplineA",
                "AreaA",
                null,
                null,
                2,
                new List<RequirementForCommand> { new RequirementForCommand(11, 12) },
                "DescriptionA",
                "RemarkA",
                "SA_A");

            Assert.AreEqual("ProjectNameA", dut.ProjectName);
            Assert.AreEqual("DisciplineA", dut.DisciplineCode);
            Assert.AreEqual("AreaA", dut.AreaCode);
            Assert.AreEqual("DescriptionA", dut.Description);
            Assert.AreEqual("RemarkA", dut.Remark);
            Assert.AreEqual("SA_A", dut.StorageArea);
            Assert.AreEqual(2, dut.StepId);
            Assert.AreEqual(1, dut.Requirements.Count());
            var requirement = dut.Requirements.First();
            Assert.AreEqual(11, requirement.RequirementDefinitionId);
            Assert.AreEqual(12, requirement.IntervalWeeks);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WithNullLists()
        {
            var dut = new CreateAreaTagCommand("ProjectNameA",
                TagType.PreArea,
                "DiscA",
                "AreaA",
                null,
                null,
                2,
                null,
                null,
                null,
                null);

            Assert.IsNotNull(dut.Requirements);
            Assert.AreEqual(0, dut.Requirements.Count());
            Assert.IsNull(dut.Description);
            Assert.IsNull(dut.Remark);
            Assert.IsNull(dut.StorageArea);
        }
    }
}
