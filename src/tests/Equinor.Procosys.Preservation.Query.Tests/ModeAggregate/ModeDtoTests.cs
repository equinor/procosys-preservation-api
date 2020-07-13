using Equinor.Procosys.Preservation.Query.ModeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.ModeAggregate
{
    [TestClass]
    public class ModeDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new ModeDto(3, "M", true, true, true,"AAAAAAAAABA=");

            Assert.AreEqual(3, dut.Id);
            Assert.AreEqual("M", dut.Title);
            Assert.IsTrue(dut.IsVoided);
            Assert.IsTrue(dut.InUse);
            Assert.IsTrue(dut.ForSupplier);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
