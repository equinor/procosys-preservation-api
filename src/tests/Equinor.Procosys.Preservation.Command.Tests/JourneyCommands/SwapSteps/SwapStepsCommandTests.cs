using Equinor.Procosys.Preservation.Command.JourneyCommands.SwapSteps;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.SwapSteps
{
    [TestClass]
    public class SwapStepsCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new SwapStepsCommand(1, 2, "AAAAAAAAABA=", 3, "AAAAAAAAABB=");

            Assert.AreEqual(1, dut.JourneyId);
            Assert.AreEqual(2, dut.StepAId);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersionA);
            Assert.AreEqual(3, dut.StepBId);
            Assert.AreEqual("AAAAAAAAABB=", dut.RowVersionB);
        }
    }
}
