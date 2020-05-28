using Equinor.Procosys.Preservation.Query.GetModes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetModes
{
    [TestClass]
    public class ModeDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new ModeDto(3, "M", "AAAAAAAAABA=");

            Assert.AreEqual(3, dut.Id);
            Assert.AreEqual("M", dut.Title);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
