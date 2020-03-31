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
            var dut = new TransferCommand("", new List<int>{17});

            Assert.AreEqual(1, dut.TagIds.Count());
            Assert.AreEqual(17, dut.TagIds.First());
        }
    }
}
