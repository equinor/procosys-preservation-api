using System;
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
            var dut = new SwapStepsCommand(1, stepA.Id, stepA.RowVersion, stepB.Id, stepB.RowVersion, Guid.Empty);

            Assert.AreEqual(1, dut.JourneyId);
            Assert.AreEqual(2, dut.StepAId);
            Assert.AreEqual("AAAAAAAAABA=", dut.StepARowVersion);
            Assert.AreEqual(3, dut.StepBId);
            Assert.AreEqual("AAAAAAAACBA=", dut.StepBRowVersion);
            Assert.AreEqual(Guid.Empty, dut.CurrentUserOid);
        }
    }
}
