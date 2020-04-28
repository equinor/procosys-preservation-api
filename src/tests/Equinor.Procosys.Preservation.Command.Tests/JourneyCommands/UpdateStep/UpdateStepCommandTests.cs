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
            var dut = new UpdateStepCommand(1, "TitleNew");

            Assert.AreEqual(1, dut.StepId);
            Assert.AreEqual("TitleNew", dut.Title);
        }
    }
}
