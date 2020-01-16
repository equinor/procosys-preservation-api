using Equinor.Procosys.Preservation.Command.TagCommands.SetStep;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.SetStep
{
    [TestClass]
    public class SetStepCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new SetStepCommand(1, 2);

            Assert.AreEqual(1, dut.TagId);
            Assert.AreEqual(2, dut.StepId);
        }
    }
}
