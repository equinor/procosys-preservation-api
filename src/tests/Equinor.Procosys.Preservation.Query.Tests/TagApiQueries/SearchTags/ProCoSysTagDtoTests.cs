using Equinor.ProCoSys.Preservation.Query.TagApiQueries.SearchTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.TagApiQueries.SearchTags
{
    [TestClass]
    public class ProCoSysTagDtoTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new ProcosysTagDto("TagNo", "Desc", "PoNo", "CommPkgNo", "McPkgNo", "TFC", "RC", "R1,R2", true);

            Assert.AreEqual("TagNo", dut.TagNo);
            Assert.AreEqual("Desc", dut.Description);
            Assert.AreEqual("PoNo", dut.PurchaseOrderTitle);
            Assert.AreEqual("CommPkgNo", dut.CommPkgNo);
            Assert.AreEqual("McPkgNo", dut.McPkgNo);
            Assert.AreEqual("McPkgNo", dut.McPkgNo);
            Assert.AreEqual("TFC", dut.TagFunctionCode);
            Assert.AreEqual("RC", dut.RegisterCode);
            Assert.AreEqual("R1,R2", dut.MccrResponsibleCodes);
            Assert.IsTrue(dut.IsPreserved);
        }
    }
}
