using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.ResponsibleAggregate
{
    [TestClass]
    public class ResponsibleDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new ResponsibleDto(3, "RC", "RD", "AAAAAAAAABA=");

            Assert.AreEqual(3, dut.Id);
            Assert.AreEqual("RC", dut.Code);
            Assert.AreEqual("RT", dut.Description);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
