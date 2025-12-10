using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.TagCommands.StartPreservation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.StartPreservation
{
    [TestClass]
    public class StartPreservationCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new StartPreservationCommand(new List<int> { 17 });

            Assert.AreEqual(1, dut.TagIds.Count());
            Assert.AreEqual(17, dut.TagIds.First());
        }
    }
}
