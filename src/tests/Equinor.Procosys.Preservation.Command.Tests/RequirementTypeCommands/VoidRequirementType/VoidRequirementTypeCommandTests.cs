using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.VoidRequirementType;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementTypeCommands.VoidRequirementType
{
    [TestClass]
    public class VoidRequirementTypeCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new VoidRequirementTypeCommand(1, "AAAAAAAAABA=");

            Assert.AreEqual(1, dut.RequirementTypeId);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
