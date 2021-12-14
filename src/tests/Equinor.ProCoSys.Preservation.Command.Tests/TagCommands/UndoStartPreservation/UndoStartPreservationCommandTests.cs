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
            var idAndRowVersion = new IdAndRowVersion(17, "AAAAAAAAABA=");
            var dut = new UndoStartPreservationCommand(new List<IdAndRowVersion> { idAndRowVersion });

            Assert.AreEqual(1, dut.Tags.Count());
            Assert.AreEqual(idAndRowVersion, dut.Tags.First());
        }
    }
}
