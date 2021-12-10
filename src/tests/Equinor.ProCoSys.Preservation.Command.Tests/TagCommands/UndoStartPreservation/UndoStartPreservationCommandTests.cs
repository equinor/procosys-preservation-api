using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UndoStartPreservation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.UndoStartPreservation
{
    [TestClass]
    public class UndoStartPreservationCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UndoStartPreservationCommand(new List<int>{17});

            Assert.AreEqual(1, dut.TagIds.Count());
            Assert.AreEqual(17, dut.TagIds.First());
        }
    }
}
