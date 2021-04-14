using Equinor.ProCoSys.Preservation.Command.JourneyCommands.CreateJourney;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.CreateJourney
{
    [TestClass]
    public class CreateJourneyCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new CreateJourneyCommand("TitleA");

            Assert.AreEqual("TitleA", dut.Title);
        }
    }
}
