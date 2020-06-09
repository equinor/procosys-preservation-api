using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.CreateTags
{
    [TestClass]
    public class CreateTagsCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties_WithRequirements()
        {
            var dut = new CreateTagsCommand(
                new List<string> {"TagNoA"},
                "ProjectNameA",
                2,
                new List<RequirementForCommand>{new RequirementForCommand(11, 12)},
                "RemarkA",
                "SA_A",
                Guid.Empty);

            Assert.AreEqual("ProjectNameA", dut.ProjectName);
            Assert.AreEqual("RemarkA", dut.Remark);
            Assert.AreEqual("SA_A", dut.StorageArea);
            Assert.AreEqual(2, dut.StepId);
            Assert.AreEqual(1, dut.Requirements.Count());
            var requirement = dut.Requirements.First();
            Assert.AreEqual(11, requirement.RequirementDefinitionId);
            Assert.AreEqual(12, requirement.IntervalWeeks);
            Assert.AreEqual(1, dut.TagNos.Count());
            Assert.AreEqual("TagNoA", dut.TagNos.First());
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WithNullLists()
        {
            var dut = new CreateTagsCommand(null, "", 0, null, null, null, Guid.Empty);

            Assert.IsNotNull(dut.Requirements);
            Assert.AreEqual(0, dut.Requirements.Count());
            Assert.IsNotNull(dut.TagNos);
            Assert.AreEqual(0, dut.TagNos.Count());
            Assert.IsNull(dut.Remark);
            Assert.IsNull(dut.StorageArea);
            Assert.AreEqual(Guid.Empty, dut.CurrentUserOid);
        }
    }
}
