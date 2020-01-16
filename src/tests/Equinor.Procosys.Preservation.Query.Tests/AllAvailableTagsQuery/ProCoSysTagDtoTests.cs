using Equinor.Procosys.Preservation.Query.AllAvailableTagsQuery;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.AllAvailableTagsQuery
{
    [TestClass]
    public class ProCoSysTagDtoTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new ProcosysTagDto("TagNo", "Desc", "PoNo", "CommPkgNo", "McPkgNo", true);

            Assert.AreEqual("TagNo", dut.TagNo);
            Assert.AreEqual("Desc", dut.Description);
            Assert.AreEqual("PoNo", dut.PurchaseOrderNumber);
            Assert.AreEqual("CommPkgNo", dut.CommPkgNo);
            Assert.AreEqual("McPkgNo", dut.McPkgNo);
            Assert.IsTrue(dut.IsPreserved);
        }
    }
}
