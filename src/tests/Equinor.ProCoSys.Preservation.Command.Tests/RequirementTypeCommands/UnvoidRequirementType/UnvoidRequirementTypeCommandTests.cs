using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementType;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementTypeCommands.UnvoidRequirementType
{
    [TestClass]
    public class UnvoidRequirementTypeCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UnvoidRequirementTypeCommand(1, "AAAAAAAAABA=");

            Assert.AreEqual(1, dut.RequirementTypeId);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
