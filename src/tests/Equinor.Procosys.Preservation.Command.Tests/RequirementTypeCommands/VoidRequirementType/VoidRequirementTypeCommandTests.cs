using System;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.VoidRequirementType;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.VoidRequirementType
{
    [TestClass]
    public class VoidRequirementTypeCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new VoidRequirementTypeCommand(1, "AAAAAAAAABA=", Guid.Empty);

            Assert.AreEqual(1, dut.RequirementTypeId);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
            Assert.AreEqual(Guid.Empty, dut.CurrentUserOid);
        }
    }
}
