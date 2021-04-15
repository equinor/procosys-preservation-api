using Equinor.ProCoSys.Preservation.Command.JourneyCommands.VoidJourney;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.VoidJourney
{
    [TestClass]
    public class VoidJourneyCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new VoidJourneyCommand(2, "AAAAAAAAABA=");

            Assert.AreEqual(2, dut.JourneyId);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
