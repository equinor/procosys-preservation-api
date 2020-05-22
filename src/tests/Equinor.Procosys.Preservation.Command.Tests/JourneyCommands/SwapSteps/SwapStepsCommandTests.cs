using System.Collections.Generic;
using System.Linq;
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
            var stepA = new StepIdAndRowVersion ( 2, "AAAAAAAAABA=");
            var stepB = new StepIdAndRowVersion (3, "AAAAAAAACBA=");
            var dut = new SwapStepsCommand(1, new List<StepIdAndRowVersion>{stepA, stepB});

            Assert.AreEqual(1, dut.JourneyId);
            Assert.AreEqual(2, dut.Steps.First().Id);
            Assert.AreEqual("AAAAAAAAABA=", dut.Steps.First().RowVersion);
            Assert.AreEqual(3, dut.Steps.Skip(1).First().Id);
            Assert.AreEqual("AAAAAAAACBA=", dut.Steps.Skip(1).First().RowVersion);
        }
    }
}
