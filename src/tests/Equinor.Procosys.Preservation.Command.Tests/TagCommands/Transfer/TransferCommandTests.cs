using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Command.TagCommands.Transfer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.Transfer
{
    [TestClass]
    public class TransferCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var idAndRowVersion = new IdAndRowVersion(17, "AAAAAAAAABA=");
            var dut = new TransferCommand(new List<IdAndRowVersion>{idAndRowVersion});

            Assert.AreEqual(1, dut.Tags.Count());
            Assert.AreEqual(idAndRowVersion, dut.Tags.First());
        }
    }
}
