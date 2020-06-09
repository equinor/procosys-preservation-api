using System;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.Preserve
{
    [TestClass]
    public class PreserveCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new PreserveCommand(17, Guid.Empty);

            Assert.AreEqual(17, dut.TagId);
            Assert.AreEqual(Guid.Empty, dut.CurrentUserOid);
        }
    }
}
