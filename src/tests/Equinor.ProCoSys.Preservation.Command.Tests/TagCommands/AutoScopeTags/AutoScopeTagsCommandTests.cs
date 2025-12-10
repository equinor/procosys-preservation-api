using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.TagCommands.AutoScopeTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.AutoScopeTags
{
    [TestClass]
    public class AutoScopeTagsCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties_WithRequirements()
        {
            var dut = new AutoScopeTagsCommand(
                new List<string> { "TagNoA" },
                "ProjectNameA",
                2,
                "RemarkA",
                "SA_A");

            Assert.AreEqual("ProjectNameA", dut.ProjectName);
            Assert.AreEqual("RemarkA", dut.Remark);
            Assert.AreEqual("SA_A", dut.StorageArea);
            Assert.AreEqual(2, dut.StepId);
            Assert.AreEqual(1, dut.TagNos.Count());
            Assert.AreEqual("TagNoA", dut.TagNos.First());
        }
    }
}
