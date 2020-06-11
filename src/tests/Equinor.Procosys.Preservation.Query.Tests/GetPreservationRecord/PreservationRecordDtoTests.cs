using Equinor.Procosys.Preservation.Query.GetPreservationRecord;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetPreservationRecord
{
    [TestClass]
    public class PreservationRecordDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new PreservationRecordDto(2, true, "AAAAAAAAABA=");

            Assert.AreEqual(2, dut.Id);
            Assert.IsTrue(dut.BulkPreserved);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}
