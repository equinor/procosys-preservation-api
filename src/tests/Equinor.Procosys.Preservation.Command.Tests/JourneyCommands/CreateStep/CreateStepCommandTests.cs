using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.CreateStep
{
    [TestClass]
    public class CreateStepCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new CreateStepCommand(1, 2, 3);

            Assert.AreEqual(1, dut.JourneyId);
            Assert.AreEqual(2, dut.ModeId);
            Assert.AreEqual(3, dut.ResponsibleId);
        }
    }
}
