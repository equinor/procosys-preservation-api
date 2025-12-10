using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Command.TagCommands.CompletePreservation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.CompletePreservation
{
    [TestClass]
    public class CompletePreservationCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var idAndRowVersion = new IdAndRowVersion(17, "AAAAAAAAABA=");
            var dut = new CompletePreservationCommand(new List<IdAndRowVersion> { idAndRowVersion });

            Assert.AreEqual(1, dut.Tags.Count());
            Assert.AreEqual(idAndRowVersion, dut.Tags.First());
        }
    }
}
