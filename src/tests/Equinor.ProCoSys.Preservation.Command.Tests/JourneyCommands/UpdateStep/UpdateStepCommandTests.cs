using Equinor.ProCoSys.Preservation.Command.JourneyCommands.UpdateStep;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.UpdateStep
{
    [TestClass]
    public class UpdateStepCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UpdateStepCommand(1, 2, 3, "CODE", "TitleNew", AutoTransferMethod.OnRfocSign, "AAAAAAAAABA=");

            Assert.AreEqual(1, dut.JourneyId);
            Assert.AreEqual(2, dut.StepId);
            Assert.AreEqual(3, dut.ModeId);
            Assert.AreEqual("CODE", dut.ResponsibleCode);
            Assert.AreEqual("TitleNew", dut.Title);
            Assert.AreEqual(AutoTransferMethod.OnRfocSign, dut.AutoTransferMethod);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
