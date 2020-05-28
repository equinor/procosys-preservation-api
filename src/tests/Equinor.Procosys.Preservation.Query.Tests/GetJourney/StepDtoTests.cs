using Equinor.Procosys.Preservation.Query.GetJourney;
using Equinor.Procosys.Preservation.Query.GetMode;
using Equinor.Procosys.Preservation.Query.GetResponsibles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetJourney
{
    [TestClass]
    public class StepDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            const string RowVersion = "AAAAAAAAABA=";
            var modeDto = new ModeDto(3, "M", null);
            var responsibleDto = new ResponsibleDto(4, "RC", "RT", null);

            var dut = new StepDto(2, "S", true, modeDto, responsibleDto, RowVersion);

            Assert.AreEqual(2, dut.Id);
            Assert.AreEqual("S", dut.Title);
            Assert.IsTrue(dut.IsVoided);
            Assert.AreEqual(RowVersion, dut.RowVersion);
            Assert.AreEqual(modeDto, dut.Mode);
            Assert.AreEqual(responsibleDto, dut.Responsible);
        }
    }
}
