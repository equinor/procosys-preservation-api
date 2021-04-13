using Equinor.ProCoSys.Preservation.Command.JourneyCommands.UnvoidJourney;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.UnvoidJourney
{
    [TestClass]
    public class UnvoidJourneyCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UnvoidJourneyCommand(2, "AAAAAAAAABA=");

            Assert.AreEqual(2, dut.JourneyId);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
