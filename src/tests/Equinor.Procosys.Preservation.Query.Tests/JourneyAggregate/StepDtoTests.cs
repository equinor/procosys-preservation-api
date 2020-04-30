using Equinor.Procosys.Preservation.Query.JourneyAggregate;
using Equinor.Procosys.Preservation.Query.ModeAggregate;
using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.JourneyAggregate
{
    [TestClass]
    public class StepDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var modeDto = new ModeDto(3, "M");
            var responsibleDto = new ResponsibleDto(4, "RC", "RT", 12345);

            var dut = new StepDto(2, "S", true, modeDto, responsibleDto);

            Assert.AreEqual(2, dut.Id);
            Assert.AreEqual("S", dut.Title);
            Assert.IsTrue(dut.IsVoided);
            Assert.AreEqual(modeDto, dut.Mode);
            Assert.AreEqual(responsibleDto, dut.Responsible);
        }
    }
}
