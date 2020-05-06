using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Command.TagFunctionCommands.UpdateRequirements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagFunctionCommands.UpdateRequirements
{
    [TestClass]
    public class UpdateRequirementsCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties_WithRequirements()
        {
            var req = new RequirementForCommand(1, 2);
            var dut = new UpdateRequirementsCommand("TFC", "RC", new List<RequirementForCommand> {req}, "AAAAAAAAABA=");

            Assert.AreEqual("TFC", dut.TagFunctionCode);
            Assert.AreEqual("RC", dut.RegisterCode);
            Assert.AreEqual(1, dut.Requirements.Count());
            Assert.AreEqual(req, dut.Requirements.Single());
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WithoutRequirements()
        {
            var dut = new UpdateRequirementsCommand("TFC", "RC", null, null);

            Assert.IsNotNull(dut.Requirements);
            Assert.AreEqual(0, dut.Requirements.Count());
        }
    }
}
