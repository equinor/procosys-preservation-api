using System;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UpdateStep
{
    [TestClass]
    public class UpdateStepCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UpdateStepCommand(1, 2, 3, "CODE", "TitleNew", "AAAAAAAAABA=", Guid.Empty);

            Assert.AreEqual(1, dut.JourneyId);
            Assert.AreEqual(2, dut.StepId);
            Assert.AreEqual(3, dut.ModeId);
            Assert.AreEqual("CODE", dut.ResponsibleCode);
            Assert.AreEqual("TitleNew", dut.Title);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
            Assert.AreEqual(Guid.Empty, dut.CurrentUserOid);
        }
    }
}
